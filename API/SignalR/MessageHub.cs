using API.DTO;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IUnitOfWork repo, IMapper mapper, IHubContext<PresenceHub> presenceHubContext) : Hub
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
        var messages = await repo.MessageRepository.GetMessagesThreadAsync(Context.User.GetUsername(), otherUser!);
        if (repo.HasChanges())
        {
            await repo.Complete();
        }
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public async Task SendMessage(CreateMessageDto messageDto)
    {
        var username = Context.User?.GetUsername() ?? throw new HubException("Cannot get current user claim");
        if (username == messageDto.RecipientUsername.ToLower())
        {
            throw new HubException("You cannot send messages to yourself");
        }
        var sender = await repo.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await repo.UserRepository.GetUserByUsernameAsync(messageDto.RecipientUsername);
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
        var group = await repo.MessageRepository.GetMessageGroupAsync(groupName);
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
        repo.MessageRepository.AddMessage(message);
        if (await repo.Complete())
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
        if(await repo.Complete())
            return group;
        throw new HubException("Failed to add to group");
    }

    async Task<Group> RemoveFromMessageGroup()
    {
        var group = await repo.MessageRepository.GetGroupForConnectionAsync(Context.ConnectionId);
        var connection= group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);  
        if (connection is not null && group is not null)
        {
            repo.MessageRepository.RemoveConnection(connection);
            if(await repo.Complete())
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