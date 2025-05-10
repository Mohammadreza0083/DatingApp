namespace API.DTO;

/// <summary>
/// Data transfer object for photos awaiting approval
/// Used in the photo moderation process
/// </summary>
public class PhotoForApprovalDto
{
    /// <summary>
    /// Unique identifier for the photo
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// URL of the photo
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// Username of the user who uploaded the photo
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Indicates whether the photo has been approved by moderators
    /// </summary>
    public bool IsApproved { get; set; }
}