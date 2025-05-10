namespace API.DTO;

/// <summary>
/// Data transfer object for creating new messages
/// Used when sending messages between users
/// </summary>
public class CreateMessageDto
{
    /// <summary>
    /// Username of the user who will receive the message
    /// </summary>
    public required string RecipientUsername { get; set; }

    /// <summary>
    /// Content of the message to be sent
    /// </summary>
    public required string Content { get; set; }
}