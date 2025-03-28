using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessageRepository(DataContext context, IMapper mapper): IMessageRepository
{
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<Message?> GetMessageAsync(int id)
    {
        return await context.Messages.FindAsync(id);
    }

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

    public async Task<IEnumerable<MessageDto>> GetMessagesThreadAsync
        (string currentUsername, string recipientUsername)
    {
        var messages = await context.Messages
            .Include(s => s.Sender).ThenInclude(s => s.Photos)
            .Include(r => r.Recipient).ThenInclude(r => r.Photos)
            .Where(
                x => x.RecipientUsername == currentUsername && x.RecipientDeleted == false && x.SenderUsername == recipientUsername
                     || x.RecipientUsername == recipientUsername && x.SenderUsername == currentUsername && x.SenderDeleted == false)
            .OrderBy(m => m.MessageSent)
            .ToListAsync();
        var unreadMessages = messages.Where(m => m.DateRead == null && 
                                                 m.RecipientUsername == currentUsername).ToList();
        if (unreadMessages.Count is not 0)
        {
            unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
            await context.SaveChangesAsync();
        }

        return mapper.Map<IEnumerable<MessageDto>>(messages);
    }

    public async Task<bool> SaveAllChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}