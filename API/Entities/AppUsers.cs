using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

/// <summary>
/// Entity representing a user in the application
/// Extends IdentityUser with custom properties for the dating application
/// </summary>
public class AppUsers:IdentityUser<int>
{
    /// <summary>
    /// User's date of birth
    /// </summary>
    public DateOnly DateOfBirth { get; init; }
    
    /// <summary>
    /// User's display name
    /// Maximum length of 25 characters
    /// </summary>
    [MaxLength(25)]
    public required string KnownAs { get; set; }

    /// <summary>
    /// Date when the user's account was created
    /// Defaults to the current UTC date and time
    /// </summary>
    public DateTime Created { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Date of the user's last activity
    /// Defaults to the current UTC date and time
    /// </summary>
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User's gender
    /// Maximum length of 6 characters
    /// </summary>
    [MaxLength(6)]
    public required string Gender { get; init; }

    /// <summary>
    /// User's self-introduction or bio
    /// Maximum length of 50 characters
    /// </summary>
    [MaxLength(50)]
    public string? Introduction { get; set; }

    /// <summary>
    /// User's interests and hobbies
    /// Maximum length of 50 characters
    /// </summary>
    [MaxLength(50)]
    public string? Interests { get; set; }

    /// <summary>
    /// What the user is looking for in a relationship
    /// Maximum length of 50 characters
    /// </summary>
    [MaxLength(50)]
    public string? LookingFor { get; set; }

    /// <summary>
    /// City where the user lives
    /// Maximum length of 50 characters
    /// </summary>
    [MaxLength(50)]
    public required string City { get; init; }

    /// <summary>
    /// Country where the user lives
    /// Maximum length of 50 characters
    /// </summary>
    [MaxLength(50)]
    public required string Country { get; init; }

    /// <summary>
    /// Collection of the user's photos
    /// </summary>
    public List<Photo> Photos { get; set; } = [];

    /// <summary>
    /// Collection of users who have liked this user
    /// </summary>
    public List<UserLike> LikedByUsers { get; set; } = [];

    /// <summary>
    /// Collection of users this user has liked
    /// </summary>
    public List<UserLike> LikedUsers { get; set; } = [];

    /// <summary>
    /// Collection of messages sent by this user
    /// </summary>
    public List<Message> MessagesSent { get; set; } = [];

    /// <summary>
    /// Collection of messages received by this user
    /// </summary>
    public List<Message> MessagesReceived { get; set; }= [];
    
    /// <summary>
    /// Collection of roles assigned to this user
    /// </summary>
    public ICollection<AppUserRole> UserRoles{ get; set; }= new List<AppUserRole>();
}