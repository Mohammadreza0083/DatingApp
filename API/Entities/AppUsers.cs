using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUsers:IdentityUser<int>
{
    public DateOnly DateOfBirth { get; init; }
    
    [MaxLength(25)]
    public required string KnownAs { get; set; }

    public DateTime Created { get; init; } = DateTime.UtcNow;

    public DateTime LastActivity { get; set; } = DateTime.UtcNow;

    [MaxLength(6)]
    public required string Gender { get; init; }

    [MaxLength(50)]
    public string? Introduction { get; set; }

    [MaxLength(50)]
    public string? Interests { get; set; }

    [MaxLength(50)]
    public string? LookingFor { get; set; }

    [MaxLength(50)]
    public required string City { get; init; }

    [MaxLength(50)]
    public required string Country { get; init; }

    public List<Photo> Photos { get; set; } = [];

    public List<UserLike> LikedByUsers { get; set; } = [];

    public List<UserLike> LikedUsers { get; set; } = [];

    public List<Message> MessagesSent { get; set; } = [];

    public List<Message> MessagesReceived { get; set; }= [];
    
    public ICollection<AppUserRole> UserRoles{ get; set; }= new List<AppUserRole>();
}