using System.ComponentModel.DataAnnotations;

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
    [MaxLength(100)]
    public required string ConnectionId { get; set; }

    /// <summary>
    /// Username of the connected user
    /// </summary>
    [MaxLength(100)]
    public required string Username { get; set; }
}