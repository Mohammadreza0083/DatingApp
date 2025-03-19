using System.ComponentModel.DataAnnotations;

namespace API.DTO;

public class RegisterDto
{
    [Required]
    [MinLength(3)]
    public string Username { get; set; } = string.Empty;
    
    [Required] public string? KnownAs { get; set; }
    [Required] public string? Gender { get; set; }
    [Required] public string? DateOfBirth { get; set; }
    [Required] public string? City { get; set; }
    [Required] public string? Country { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 8)]
    [RegularExpression(
        "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,15}$",
        ErrorMessage = "Password must have 1 uppercase, 1 lowercase, 1 number, and at least 8 characters"
    )]
    public string Password { get; set; } = string.Empty;
}
