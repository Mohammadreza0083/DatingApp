namespace API.DTO;

/// <summary>
/// Data transfer object for updating member profile information
/// Contains optional fields for user profile updates
/// </summary>
public class MemberUpdateDto
{
    /// <summary>
    /// User's interests and hobbies
    /// </summary>
    public string? Interests { get; set; }

    /// <summary>
    /// What the user is looking for in a relationship
    /// </summary>
    public string? LookingFor { get; set; }

    /// <summary>
    /// User's self-introduction or bio
    /// </summary>
    public string? Introduction { get; set; }

    /// <summary>
    /// City where the user lives
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Country where the user lives
    /// </summary>
    public string? Country { get; set; }
}