using System.Security.Claims;

namespace API.Extensions;

/// <summary>
/// Extension methods for ClaimsPrincipal to provide easy access to user claims
/// </summary>
public static class ClaimsPrincipalExtension
{
    /// <summary>
    /// Gets the username from the user's claims
    /// </summary>
    /// <param name="user">The ClaimsPrincipal representing the current user</param>
    /// <returns>The username as a string</returns>
    /// <exception cref="Exception">Thrown when no user is found in the claims</exception>
    public static string GetUsername(this ClaimsPrincipal user)
    {
        var username = user.FindFirstValue(ClaimTypes.Name)
            ?? throw new Exception("No user found");
        return username;
    }    
    
    /// <summary>
    /// Gets the user ID from the user's claims
    /// </summary>
    /// <param name="user">The ClaimsPrincipal representing the current user</param>
    /// <returns>The user ID as an integer</returns>
    /// <exception cref="Exception">Thrown when no user is found in the claims</exception>
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? throw new Exception("No user found"));
        return userId;
    }
}