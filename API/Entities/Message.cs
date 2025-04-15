using System.ComponentModel.DataAnnotations;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace API.Entities;

/// <summary>
/// Entity representing a message between users
/// Used for the messaging system in the dating application
/// </summary>
public class Message
{
    /// <summary>
    /// Unique identifier for the message
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Username of the sender
    /// Maximum length of 30 characters
    /// </summary>
    [MaxLength(30)]
    public required string SenderUsername { get; set; }

    /// <summary>
    /// Username of the recipient
    /// Maximum length of 30 characters
    /// </summary>
    [MaxLength(30)]
    public required string RecipientUsername { get; set; }

    /// <summary>
    /// Content of the message
    /// Maximum length of 30 characters
    /// </summary>
    [MaxLength(30)]
    public required string Content { get; set; }

    /// <summary>
    /// Date and time when the message was read
    /// Null if the message has not been read
    /// </summary>
    public DateTime? DateRead { get; set; }

    /// <summary>
    /// Date and time when the message was sent
    /// Defaults to the current date and time
    /// </summary>
    public DateTime? MessageSent { get; set; }=DateTime.Now;

    /// <summary>
    /// Indicates whether the sender has deleted the message
    /// </summary>
    public bool SenderDeleted { get; set; }

    /// <summary>
    /// Indicates whether the recipient has deleted the message
    /// </summary>
    public bool RecipientDeleted { get; set; }

    /// <summary>
    /// Navigation property to the sender
    /// </summary>
    public AppUsers Sender { get; set; } = null!;

    /// <summary>
    /// Navigation property to the recipient
    /// </summary>
    public AppUsers Recipient { get; set; } = null!;

    /// <summary>
    /// Foreign key to the sender
    /// </summary>
    public int SenderId { get; set; }

    /// <summary>
    /// Foreign key to the recipient
    /// </summary>
    public int RecipientId { get; set; }
}