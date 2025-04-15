namespace API.DTO;

/// <summary>
/// Data transfer object for user photos
/// Used for displaying photos in the user interface
/// </summary>
public class PhotoDto
{
    /// <summary>
    /// Unique identifier for the photo
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// URL of the photo
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Indicates whether this is the user's main profile photo
    /// </summary>
    public bool IsMain { get; set; }

    /// <summary>
    /// Indicates whether the photo has been approved by moderators
    /// </summary>
    public bool IsApproved { get; set; }
}