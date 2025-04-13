using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace API.Entities;

[Table("Photos")]
public class Photo
{
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")] 
  public int Id { get; set; }

    [MaxLength(100)]
    public required string Url { get; init; }

    public bool IsMain { get; set; }

    [MaxLength(10)]
    public string? PublicId { get; set; }

    public bool IsApproved { get; set; } = false;

    /// <summary>
    /// Navigation property
    /// </summary>
    public int AppUserId { get; set; }

    /// <summary>
    /// Navigation property
    /// </summary>
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public AppUsers AppUsers { get; set; } = null!;
}