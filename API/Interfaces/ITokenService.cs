using API.Entities;

namespace API.Interfaces;

/// <summary>
/// Interface for managing JWT token operations
/// </summary>
public interface ITokenServices
{
    /// <summary>
    /// Creates a JWT token for a user
    /// </summary>
    /// <param name="user">The user to create the token for</param>
    /// <returns>The generated JWT token</returns>
    Task<string> CreateToken(AppUsers user);
}
