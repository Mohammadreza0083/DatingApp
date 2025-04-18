﻿using API.DTO;
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

    public void AddGroup(Group group)
    {
        context.Groups.Add(group);
    }

    public void RemoveConnection(Connection connection)
    {
        context.Connections.Remove(connection);
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

    public async Task<Group?> GetMessageGroupAsync(string groupName)
    {
        return await context.Groups
            .Include(x=> x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<Connection?> GetConnectionAsync(string connectionId)
    {
        return await context.Connections.FindAsync(connectionId);
    }

    public async Task<Group?> GetGroupForConnectionAsync(string connectionId)
    {
        return await context.Groups
            .Include(x => x.Connections)
            .Where(x => x.Connections.Any(s => s.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }
}