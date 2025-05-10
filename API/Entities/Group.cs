using System.ComponentModel.DataAnnotations;

namespace API.Entities;

/// <summary>
/// Entity representing a chat group
/// Used for real-time messaging functionality
/// </summary>
public class Group
{
    /// <summary>
    /// Name of the group
    /// Serves as the primary key
    /// </summary>
    [Key]
    [MaxLength(50)]
    public required string Name { get; set; }

    /// <summary>
    /// Collection of active connections in the group
    /// </summary>
    public ICollection<Connection> Connections { get; set; } = [];
}