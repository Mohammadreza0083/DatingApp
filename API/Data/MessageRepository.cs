using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Repository for managing message-related operations
/// Implements IMessageRepository interface
/// </summary>
public class MessageRepository(DataContext context, IMapper mapper): IMessageRepository
{
    /// <summary>
    /// Adds a new message to the database
    /// </summary>
    /// <param name="message">The message entity to add</param>
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    /// <summary>
    /// Deletes a message from the database
    /// </summary>
    /// <param name="message">The message entity to delete</param>
    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    /// <summary>
    /// Adds a new chat group to the database
    /// </summary>
    /// <param name="group">The group entity to add</param>
    public void AddGroup(Group group)
    {
        context.Groups.Add(group);
    }

    /// <summary>
    /// Removes a SignalR connection from the database
    /// </summary>
    /// <param name="connection">The connection entity to remove</param>
    public void RemoveConnection(Connection connection)
    {
        context.Connections.Remove(connection);
    }

    /// <summary>
    /// Retrieves a message by its ID
    /// </summary>
    /// <param name="id">The ID of the message to find</param>
    /// <returns>The message if found, null otherwise</returns>
    public async Task<Message?> GetMessageAsync(int id)
    {
        return await context.Messages.FindAsync(id);
    }

    /// <summary>
    /// Retrieves a paginated list of messages for a user based on specified parameters
    /// Filters messages based on inbox, outbox, or unread status
    /// </summary>
    /// <param name="messageParams">Parameters for filtering and pagination</param>
    /// <returns>A paginated list of MessageDto objects</returns>
    public async Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams)
    {
        var query = context.Messages
            .OrderByDescending(m => m.MessageSent)
            .AsQueryable();
        query = messageParams.Container switch
        {
            "Inbox" => query.Where(m => m.Recipient.UserName == messageParams.Username && m.RecipientDeleted == false),
            "Outbox" => query.Where(m => m.Sender.UserName == messageParams.Username && m.SenderDeleted == false),
            _ => query.Where(m => m.Recipient.UserName == messageParams.Username && m.RecipientDeleted == false && m.DateRead == null)
        };
        var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);
        return await PagedList<MessageDto>.CreateAsync(messages, 
            messageParams.PageNum, messageParams.PageSize);
    }

    /// <summary>
    /// Retrieves a conversation thread between two users
    /// Marks unread messages as read
    /// </summary>
    /// <param name="currentUsername">The username of the current user</param>
    /// <param name="recipientUsername">The username of the recipient</param>
    /// <returns>A collection of MessageDto objects representing the conversation</returns>
    public async Task<IEnumerable<MessageDto>> GetMessagesThreadAsync
        (string currentUsername, string recipientUsername)
    {
        var query = context.Messages
            .Where(
                x => x.RecipientUsername == currentUsername && x.RecipientDeleted == false &&
                     x.SenderUsername == recipientUsername
                     || x.RecipientUsername == recipientUsername && x.SenderUsername == currentUsername &&
                     x.SenderDeleted == false)
            .OrderBy(m => m.MessageSent)
            .AsQueryable();
        var unreadMessages = query.Where(m => m.DateRead == null && 
                                                 m.RecipientUsername == currentUsername).ToList();
        if (unreadMessages.Count is not 0)
        {
            unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
        }

        return await query.ProjectTo<MessageDto>(mapper.ConfigurationProvider).ToListAsync();
    }

    /// <summary>
    /// Retrieves a chat group by its name
    /// Includes the group's connections
    /// </summary>
    /// <param name="groupName">The name of the group to find</param>
    /// <returns>The group if found, null otherwise</returns>
    public async Task<Group?> GetMessageGroupAsync(string groupName)
    {
        return await context.Groups
            .Include(x=> x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    /// <summary>
    /// Retrieves a SignalR connection by its ID
    /// </summary>
    /// <param name="connectionId">The ID of the connection to find</param>
    /// <returns>The connection if found, null otherwise</returns>
    public async Task<Connection?> GetConnectionAsync(string connectionId)
    {
        return await context.Connections.FindAsync(connectionId);
    }

    /// <summary>
    /// Retrieves a chat group that contains a specific connection
    /// Includes the group's connections
    /// </summary>
    /// <param name="connectionId">The ID of the connection to find the group for</param>
    /// <returns>The group if found, null otherwise</returns>
    public async Task<Group?> GetGroupForConnectionAsync(string connectionId)
    {
        return await context.Groups
            .Include(x => x.Connections)
            .Where(x => x.Connections.Any(s => s.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }
}