namespace API.DTO;

/// <summary>
/// Data transfer object for messages
/// Used for displaying messages in the user interface
/// </summary>
public class MessageDto
{
    /// <summary>
    /// Unique identifier for the message
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the user who sent the message
    /// </summary>
    public int SenderId { get; set; }

    /// <summary>
    /// Username of the user who sent the message
    /// </summary>
    public required string SenderUsername { get; set; }

    /// <summary>
    /// URL of the sender's profile photo
    /// </summary>
    public required string SenderPhotoUrl { get; set; }

    /// <summary>
    /// ID of the user who received the message
    /// </summary>
    public int RecipientId { get; set; }

    /// <summary>
    /// Username of the user who received the message
    /// </summary>
    public required string RecipientUsername { get; set; }

    /// <summary>
    /// URL of the recipient's profile photo
    /// </summary>
    public required string RecipientPhotoUrl { get; set; }

    /// <summary>
    /// Content of the message
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Date and time when the message was read
    /// Null if the message has not been read
    /// </summary>
    public DateTime? DateRead { get; set; }

    /// <summary>
    /// Date and time when the message was sent
    /// </summary>
    public DateTime? MessageSent { get; set; }
}