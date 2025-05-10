namespace API.DTO;

/// <summary>
/// Data transfer object for member profiles
/// Contains comprehensive user profile information
/// </summary>
public class MembersDto
{
    /// <summary>
    /// Unique identifier for the member
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// Username of the member
    /// </summary>
    public string? Username { get; set; }
    
    /// <summary>
    /// URL of the member's main profile photo
    /// </summary>
    public string? PhotoUrl { get; set; }
    
    /// <summary>
    /// Age of the member
    /// </summary>
    public int Age { get; init; }
    
    /// <summary>
    /// Display name of the member
    /// </summary>
    public string? KnownAs { get; set; }

    /// <summary>
    /// Date when the member's account was created
    /// </summary>
    public DateTime Created { get; init; }

    /// <summary>
    /// Date of the member's last activity
    /// </summary>
    public DateTime LastActivity { get; set; } 
    
    /// <summary>
    /// Gender of the member
    /// </summary>
    public string? Gender { get; init; }
    
    /// <summary>
    /// Member's self-introduction or bio
    /// </summary>
    public string? Introduction { get; set; }
    
    /// <summary>
    /// Member's interests and hobbies
    /// </summary>
    public string? Interests { get; set; }

    /// <summary>
    /// What the member is looking for in a relationship
    /// </summary>
    public string? LookingFor { get; set; }

    /// <summary>
    /// City where the member lives
    /// </summary>
    public string? City { get; init; }

    /// <summary>
    /// Country where the member lives
    /// </summary>
    public string? Country { get; init; }

    /// <summary>
    /// Collection of the member's photos
    /// </summary>
    public List<PhotoDto>? Photos { get; set; }
}