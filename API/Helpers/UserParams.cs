namespace API.Helpers;

/// <summary>
/// Parameters for filtering and paginating user queries
/// </summary>
public class UserParams : PaginationParams
{
    /// <summary>
    /// Gets or sets the gender to filter users by
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// Gets or sets the current user's username to exclude from results
    /// </summary>
    public string? CurrentUsername { get; set; }

    /// <summary>
    /// Gets or sets the minimum age for filtering users
    /// </summary>
    public int MinAge { get; set; } = 18;
    
    /// <summary>
    /// Gets or sets the maximum age for filtering users
    /// </summary>
    public int MaxAge { get; set; } = 100;

    /// <summary>
    /// Gets or sets the field to order results by
    /// </summary>
    public string? OrderBy { get; set; }  = "lastActive";
}