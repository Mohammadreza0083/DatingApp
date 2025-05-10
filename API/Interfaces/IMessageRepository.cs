using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

/// <summary>
/// Interface for managing messages and message groups
/// </summary>
public interface IMessageRepository
{
    /// <summary>
    /// Adds a new message to the repository
    /// </summary>
    /// <param name="message">The message to add</param>
    void AddMessage(Message message);

    /// <summary>
    /// Deletes a message from the repository
    /// </summary>
    /// <param name="message">The message to delete</param>
    void DeleteMessage(Message message);

    /// <summary>
    /// Adds a new message group to the repository
    /// </summary>
    /// <param name="group">The group to add</param>
    void AddGroup(Group group);

    /// <summary>
    /// Removes a connection from a message group
    /// </summary>
    /// <param name="connection">The connection to remove</param>
    void RemoveConnection(Connection connection);

    /// <summary>
    /// Gets a message by its ID
    /// </summary>
    /// <param name="id">The ID of the message</param>
    /// <returns>The message if found, null otherwise</returns>
    Task<Message?> GetMessageAsync(int id);

    /// <summary>
    /// Gets a paginated list of messages for a user
    /// </summary>
    /// <param name="messageParams">Parameters for filtering and pagination</param>
    /// <returns>A paginated list of message DTOs</returns>
    Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams);

    /// <summary>
    /// Gets the message thread between two users
    /// </summary>
    /// <param name="currentUsername">The username of the current user</param>
    /// <param name="recipientUsername">The username of the recipient</param>
    /// <returns>A list of message DTOs</returns>
    Task<IEnumerable<MessageDto>> GetMessagesThreadAsync(string currentUsername, string recipientUsername);

    /// <summary>
    /// Gets a message group by its name
    /// </summary>
    /// <param name="groupName">The name of the group</param>
    /// <returns>The group if found, null otherwise</returns>
    Task<Group?> GetMessageGroupAsync(string groupName);

    /// <summary>
    /// Gets a connection by its ID
    /// </summary>
    /// <param name="connectionId">The ID of the connection</param>
    /// <returns>The connection if found, null otherwise</returns>
    Task<Connection?> GetConnectionAsync(string connectionId);

    /// <summary>
    /// Gets the group that contains a specific connection
    /// </summary>
    /// <param name="connectionId">The ID of the connection</param>
    /// <returns>The group if found, null otherwise</returns>
    Task<Group?> GetGroupForConnectionAsync(string connectionId);
}