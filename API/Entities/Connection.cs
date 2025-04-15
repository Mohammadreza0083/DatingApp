namespace API.Entities;

/// <summary>
/// Entity representing a SignalR connection
/// Used for real-time messaging functionality
/// </summary>
public class Connection
{
    /// <summary>
    /// Unique identifier for the SignalR connection
    /// </summary>
    public required string ConnectionId { get; set; }

    /// <summary>
    /// Username of the connected user
    /// </summary>
    public required string Username { get; set; }
}