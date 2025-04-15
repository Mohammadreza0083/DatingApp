namespace API.Helpers;

/// <summary>
/// Parameters for filtering and paginating messages
/// </summary>
public class MessageParams : PaginationParams
{
    /// <summary>
    /// Gets or sets the username of the message recipient or sender
    /// </summary>
    public string? Username { get; set; }
    
    /// <summary>
    /// Gets or sets the message container type (e.g., "Unread", "Inbox", "Outbox")
    /// </summary>
    public string Container { get; set; } = "Unread";
}