using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace API.Entities;

/// <summary>
/// Entity representing a user's photo
/// Maps to the Photos table in the database
/// </summary>
[Table("Photos")]
public class Photo
{
    /// <summary>
    /// Unique identifier for the photo
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")] 
    public int Id { get; set; }

    /// <summary>
    /// URL of the photo
    /// Maximum length of 100 characters
    /// </summary>
    [MaxLength(100)]
    public required string Url { get; init; }

    /// <summary>
    /// Indicates whether this is the user's main profile photo
    /// </summary>
    public bool IsMain { get; set; }

    /// <summary>
    /// Public ID of the photo in the cloud storage
    /// Maximum length of 10 characters
    /// </summary>
    [MaxLength(10)]
    public string? PublicId { get; set; }

    /// <summary>
    /// Indicates whether the photo has been approved by moderators
    /// Defaults to false
    /// </summary>
    public bool IsApproved { get; set; } = false;

    /// <summary>
    /// Foreign key to the user who owns this photo
    /// </summary>
    public int AppUserId { get; set; }

    /// <summary>
    /// Navigation property to the user who owns this photo
    /// </summary>
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public AppUsers AppUsers { get; set; } = null!;
}