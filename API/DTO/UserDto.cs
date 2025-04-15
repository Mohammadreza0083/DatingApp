using System.ComponentModel.DataAnnotations;

namespace API.DTO;

/// <summary>
/// Data transfer object for user information
/// Used for authentication and user profile data
/// </summary>
public class UserDto
{
    /// <summary>
    /// Username of the user
    /// </summary>
    [Required]
    public required string Username { get; set; }
    
    /// <summary>
    /// Display name of the user
    /// </summary>
    [Required]
    public required string KnownAs{ get; set; }

    /// <summary>
    /// JWT token for authentication
    /// </summary>
    [Required]
    public required string Token { get; set; }
    
    /// <summary>
    /// Gender of the user
    /// </summary>
    [Required] 
    public required string Gender { get; set; }

    /// <summary>
    /// URL of the user's profile photo
    /// Null if no photo is set
    /// </summary>
    public string? PhotoUrl { get; set; }
}
