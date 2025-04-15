using API.DTO;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

/// <summary>
/// SignalR hub for managing real-time messaging
/// </summary>
public class MessageHub(IUnitOfWork repo, IMapper mapper, IHubContext<PresenceHub> presenceHubContext) : Hub
{
    /// <summary>
    /// Handles client connection to the hub
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        // Validate user context
        if (Context.User is null)
        {
            throw new HubException("Cannot get current user claim");
        }

        var httpContext = Context.GetHttpContext();
        if (httpContext is null)
        {
            throw new HubException("Unable to get HttpContext");
        }

        // Get other user from query parameters
        var otherUser = httpContext.Request.Query["user"];
        if (string.IsNullOrEmpty(otherUser))
        {
            throw new HubException("No user id provided");
        }

        // Add to message group
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        // Get and send message thread
        var messages = await repo.MessageRepository.GetMessagesThreadAsync(Context.User.GetUsername(), otherUser!);
        if (repo.HasChanges())
        {
            await repo.Complete();
        }
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    /// <summary>
    /// Sends a message to another user
    /// </summary>
    /// <param name="messageDto">The message to send</param>
    /// <exception cref="HubException">Thrown when message sending fails</exception>
    public async Task SendMessage(CreateMessageDto messageDto)
    {
        // Validate sender and recipient
        var username = Context.User?.GetUsername() ?? throw new HubException("Cannot get current user claim");
        if (username == messageDto.RecipientUsername.ToLower())
        {
            throw new HubException("You cannot send messages to yourself");
        }

        // Get sender and recipient
        var sender = await repo.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await repo.UserRepository.GetUserByUsernameAsync(messageDto.RecipientUsername);
        if (recipient is null || sender?.UserName is null || recipient.UserName is null)
        {
            throw new HubException("User not found");
        }

        // Create and save message
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = messageDto.Content
        };

        // Handle message read status
        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await repo.MessageRepository.GetMessageGroupAsync(groupName);
        if (group is not null && group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            // Notify recipient if online
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if (connections.Count is not 0)
            {
                await presenceHubContext.Clients.Clients(connections).SendAsync("NewMessageReceive",
                    new { username = recipient.UserName, message = messageDto.Content });
            }
        }

        // Save and broadcast message
        repo.MessageRepository.AddMessage(message);
        if (await repo.Complete())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
        }
        throw new HubException("Failed to send message");
    }

    /// <summary>
    /// Handles client disconnection from the hub
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Adds a user to a message group
    /// </summary>
    /// <param name="groupName">The name of the group to add to</param>
    /// <returns>The updated group</returns>
    /// <exception cref="HubException">Thrown when adding to group fails</exception>
    async Task<Group> AddToGroup(string groupName)
    {
        var username = Context.User?.GetUsername() ?? throw new HubException("Cannot get current user claim");
        var group = await repo.MessageRepository.GetMessageGroupAsync(groupName);
        var connection = new Connection()
        {
            ConnectionId = Context.ConnectionId,
            Username = username
        };

        if (group is null)
        {
            group = new Group
            {
                Name = groupName
            };
            repo.MessageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);
        if (await repo.Complete())
            return group;
        throw new HubException("Failed to add to group");
    }

    /// <summary>
    /// Removes a user from their message group
    /// </summary>
    /// <returns>The updated group</returns>
    /// <exception cref="HubException">Thrown when removing from group fails</exception>
    async Task<Group> RemoveFromMessageGroup()
    {
        var group = await repo.MessageRepository.GetGroupForConnectionAsync(Context.ConnectionId);
        var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        if (connection is not null && group is not null)
        {
            repo.MessageRepository.RemoveConnection(connection);
            if (await repo.Complete())
                return group;
        }
        throw new HubException("Failed to remove from group");
    }

    /// <summary>
    /// Generates a consistent group name for two users
    /// </summary>
    /// <param name="callerId">The first user's ID</param>
    /// <param name="other">The second user's ID</param>
    /// <returns>The generated group name</returns>
    string GetGroupName(string callerId, string? other)
    {
        var stringCompare = string.CompareOrdinal(callerId, other) < 0;
        return stringCompare ? $"{callerId}-{other}" : $"{other}-{callerId}";
    }
}