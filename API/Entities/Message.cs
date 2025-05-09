﻿using System.ComponentModel.DataAnnotations;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace API.Entities;

public class Message
{
    public int Id { get; set; }
    [MaxLength(30)]
    public required string SenderUsername { get; set; }
    [MaxLength(30)]
    public required string RecipientUsername { get; set; }
    [MaxLength(30)]
    public required string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime? MessageSent { get; set; }=DateTime.Now;
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }
    //Navigation properties
    public AppUsers Sender { get; set; } = null!;
    public AppUsers Recipient { get; set; } = null!;
    public int SenderId { get; set; }
    public int RecipientId { get; set; }
}