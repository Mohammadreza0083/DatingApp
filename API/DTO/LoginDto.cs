using System.ComponentModel.DataAnnotations;

namespace API.DTO;

/// <summary>
/// Data transfer object for user login
/// Contains validation attributes for user credentials
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Username for authentication
    /// </summary>
    [Required]
    public required string Username { get; set; }

    /// <summary>
    /// Password for authentication
    /// </summary>
    [Required]
    public required string Password { get; set; }
}
