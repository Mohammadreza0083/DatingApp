using Microsoft.AspNetCore.Identity;

namespace API.Entities;

/// <summary>
/// Entity representing a role in the application
/// Extends IdentityRole with custom properties for the dating application
/// </summary>
public class AppRole: IdentityRole<int>
{
    /// <summary>
    /// Collection of users assigned to this role
    /// </summary>
    public ICollection<AppUserRole> UserRoles{ get; set; }= new List<AppUserRole>();
    
}