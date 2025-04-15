using Microsoft.AspNetCore.Identity;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace API.Entities;

/// <summary>
/// Entity representing the many-to-many relationship between users and roles
/// Extends IdentityUserRole with custom properties for the dating application
/// </summary>
public class AppUserRole:IdentityUserRole<int>
{
    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public AppUsers Users { get; set; }= null!;

    /// <summary>
    /// Navigation property to the role
    /// </summary>
    public AppRole Roles { get; set; }= null!;
}