// ReSharper disable NullableWarningSuppressionIsUsed
namespace API.Entities;

/// <summary>
/// Entity representing a like relationship between users
/// Used for tracking user likes in the dating application
/// </summary>
public class UserLike
{
    /// <summary>
    /// Navigation property to the user who liked
    /// </summary>
    public AppUsers SourceUser { get; set; } = null!;

    /// <summary>
    /// Foreign key to the user who liked
    /// </summary>
    public int SourceUserId { get; set; }

    /// <summary>
    /// Navigation property to the user who was liked
    /// </summary>
    public AppUsers TargetUser { get; set; } = null!;
    
    /// <summary>
    /// Foreign key to the user who was liked
    /// </summary>
    public int TargetUserId { get; set; }
}