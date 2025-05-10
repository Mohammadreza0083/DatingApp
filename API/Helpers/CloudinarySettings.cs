namespace API.Helpers;

/// <summary>
/// Configuration settings for Cloudinary service integration
/// </summary>
public class CloudinarySettings
{
    /// <summary>
    /// Gets or sets the Cloudinary cloud name
    /// </summary>
    public required string CloudName { get; set; }

    /// <summary>
    /// Gets or sets the Cloudinary API key
    /// </summary>
    public required string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the Cloudinary API secret
    /// </summary>
    public required string ApiSecret { get; set; }
}