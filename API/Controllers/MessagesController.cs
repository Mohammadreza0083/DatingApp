﻿using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controller for managing user messages
/// Handles message creation, retrieval, and deletion
/// </summary>
[Authorize]
public class MessagesController(IUnitOfWork repo, IMapper mapper) : BaseApiController
{
    /// <summary>
    /// Creates a new message between two users
    /// </summary>
    /// <param name="createMessageDto">Message content and recipient information</param>
    /// <returns>Created message DTO if successful, BadRequest if failed</returns>
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessageAsync(CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername();
        if (username == createMessageDto.RecipientUsername.ToLower())
        {
            return BadRequest("You cannot send messages to yourself");
        }
        var sender = await repo.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await repo.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
        if (recipient is null || sender?.UserName is null || recipient.UserName is null)
        {
            return BadRequest("User not found");
        }
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };
        repo.MessageRepository.AddMessage(message);
        if (await repo.Complete())
        {
            return Ok(mapper.Map<MessageDto>(message));
        }
        return BadRequest("Failed to send message");
    }

    /// <summary>
    /// Retrieves paginated messages for the current user
    /// </summary>
    /// <param name="messageParams">Pagination and filtering parameters</param>
    /// <returns>List of message DTOs with pagination information</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUserAsync(
        [FromQuery]MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();
        var messages = await repo.MessageRepository.GetMessagesForUserAsync(messageParams);
        Response.AddPaginationHeader(messages);
        return messages;
    }

    /// <summary>
    /// Retrieves the message thread between the current user and another user
    /// </summary>
    /// <param name="username">Username of the other participant</param>
    /// <returns>List of message DTOs in the conversation thread</returns>
    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesThreadAsync(string username)
    {
        var currentUserUsername = User.GetUsername();
        
        return Ok(await repo.MessageRepository.GetMessagesThreadAsync(currentUserUsername, username));
    }

    /// <summary>
    /// Deletes a message for the current user
    /// Messages are only physically deleted when both sender and recipient have deleted them
    /// </summary>
    /// <param name="id">ID of the message to delete</param>
    /// <returns>Ok if successful, BadRequest if failed</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessageAsync(int id)
    {
        var currentUserUsername = User.GetUsername();
        var message = await repo.MessageRepository.GetMessageAsync(id);
        if (message is null)
        {
            return BadRequest("Message not found");
        }
        if (message.SenderUsername != currentUserUsername && 
            message.RecipientUsername != currentUserUsername)
        {
            return Forbid();
        }
        if (message.SenderUsername == currentUserUsername)
        {
            message.SenderDeleted = true;
        }
        
        if(message.RecipientUsername == currentUserUsername)
        {
            message.RecipientDeleted = true;
        }

        if (message is {SenderDeleted:true, RecipientDeleted:true})
        {
            repo.MessageRepository.DeleteMessage(message);
        }
        
        if (await repo.Complete())
        {
            return Ok();
        }
        
        return BadRequest("Problem deleting the message");
    }
}