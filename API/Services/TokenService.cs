using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

/// <summary>
/// Service for managing JWT token operations
/// </summary>
public class TokenService(IConfiguration configuration, UserManager<AppUsers> manager) : ITokenServices
{
    /// <summary>
    /// Creates a JWT token for a user
    /// </summary>
    /// <param name="user">The user to create the token for</param>
    /// <returns>The generated JWT token</returns>
    /// <exception cref="InvalidOperationException">Thrown when TokenKey is missing or too short</exception>
    /// <exception cref="ArgumentNullException">Thrown when user's username is null</exception>
    public async Task<string> CreateToken(AppUsers user)
    {
        // Get and validate token key from configuration
        string tokenKey = configuration["TokenKey"] ?? throw new InvalidOperationException("TokenKey is missing");

        if (tokenKey.Length < 64)
            throw new InvalidOperationException("TokenKey must be at least 64 characters long");

        // Create signing key
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(tokenKey));
        
        // Validate username
        if (user.UserName is null)
        {
            throw new ArgumentNullException(nameof(user.UserName), "UserName cannot be null");
        }

        // Create claims for the token
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName)
        ];

        // Add role claims
        var roles = await manager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Create signing credentials
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);

        // Configure token descriptor
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(3),
            SigningCredentials = credentials
        };

        // Create and return the token
        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
