using System.ComponentModel.DataAnnotations;

namespace API.DTO;

/// <summary>
/// Data transfer object for user registration
/// Contains validation attributes for user input
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// Username for the new account
    /// Must be at least 3 characters long
    /// </summary>
    [Required]
    [MinLength(3)]
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name for the user
    /// </summary>
    [Required] public string? KnownAs { get; set; }

    /// <summary>
    /// Gender of the user
    /// </summary>
    [Required] public string? Gender { get; set; }

    /// <summary>
    /// Date of birth of the user
    /// </summary>
    [Required] public string? DateOfBirth { get; set; }

    /// <summary>
    /// City where the user lives
    /// </summary>
    [Required] public string? City { get; set; }

    /// <summary>
    /// Country where the user lives
    /// </summary>
    [Required] public string? Country { get; set; }

    /// <summary>
    /// Password for the new account
    /// Must be between 8 and 20 characters
    /// Must contain at least one uppercase letter, one lowercase letter, and one number
    /// </summary>
    [Required]
    [StringLength(20, MinimumLength = 8)]
    [RegularExpression(
        "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,15}$",
        ErrorMessage = "Password must have 1 uppercase, 1 lowercase, 1 number, and at least 8 characters"
    )]
    public string Password { get; set; } = string.Empty;
}
