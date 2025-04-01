using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Message?> GetMessageAsync(int id);
    Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessagesThreadAsync(string currentUsername, string recipientUsername);
    Task<bool> SaveAllChangesAsync();
    Task<Group?> GetMessageGroupAsync(string groupName);
    Task<Connection?> GetConnectionAsync(string connectionId);
    Task<Group?> GetGroupForConnectionAsync(string connectionId);
}