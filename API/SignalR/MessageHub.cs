using API.DTO;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IMessageRepository messageRepository, IUserRepository userRepository,
    IMapper mapper, IHubContext<PresenceHub> presenceHubContext) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User is null)
        {
            throw new HubException("Cannot get current user claim");
        }
        var httpContext = Context.GetHttpContext();
        if (httpContext is null)
        {
            throw new HubException("Unable to get HttpContext");
        }
        var otherUser = httpContext.Request.Query["user"];
        if (string.IsNullOrEmpty(otherUser))
        {
            throw new HubException("No user id provided");
        }
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group =await AddToGroup(groupName);
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
        // ReSharper disable once NullableWarningSuppressionIsUsed
        var messages = await messageRepository.GetMessagesThreadAsync(Context.User.GetUsername(), otherUser!);
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public async Task SendMessage(CreateMessageDto messageDto)
    {
        var username = Context.User?.GetUsername() ?? throw new HubException("Cannot get current user claim");
        if (username == messageDto.RecipientUsername.ToLower())
        {
            throw new HubException("You cannot send messages to yourself");
        }
        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(messageDto.RecipientUsername);
        if (recipient is null || sender?.UserName is null || recipient.UserName is null)
        {
            throw new HubException("User not found");
        }
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = messageDto.Content
        };
        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await messageRepository.GetMessageGroupAsync(groupName);
        if (group is not null 
            && group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if (connections.Count is not 0)
            {
                await presenceHubContext.Clients.Clients(connections).SendAsync("NewMessageReceive", 
                    new {username = recipient.UserName, message = messageDto.Content});
            }
        }
        messageRepository.AddMessage(message);
        if (await messageRepository.SaveAllChangesAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
        }
        throw new HubException("Failed to send message");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }

    async Task<Group> AddToGroup(string groupName)
    {
        var username = Context.User?.GetUsername() ?? throw new HubException("Cannot get current user claim");
        var group = await messageRepository.GetMessageGroupAsync(groupName);
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
            messageRepository.AddGroup(group);
        }
        group.Connections.Add(connection);
        if(await messageRepository.SaveAllChangesAsync())
            return group;
        throw new HubException("Failed to add to group");
    }

    async Task<Group> RemoveFromMessageGroup()
    {
        var group = await messageRepository.GetGroupForConnectionAsync(Context.ConnectionId);
        var connection= group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);  
        if (connection is not null && group is not null)
        {
            messageRepository.RemoveConnection(connection);
            if(await messageRepository.SaveAllChangesAsync())
                return group;
        }
        throw new HubException("Failed to remove from group");
    }
    string GetGroupName(string callerId, string? other)
    {
        var stringCompare = string.CompareOrdinal(callerId, other) < 0;
        return stringCompare ? $"{callerId}-{other}" : $"{other}-{callerId}";
    }
}