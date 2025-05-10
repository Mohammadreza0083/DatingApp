namespace API.Helpers;

/// <summary>
/// Parameters for filtering and paginating user likes
/// </summary>
public class LikesParams : PaginationParams
{
    /// <summary>
    /// Gets or sets the ID of the user whose likes are being queried
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the predicate for filtering likes (e.g., "liked" or "likedBy")
    /// </summary>
    public required string Predicate { get; set; } = "liked";
}