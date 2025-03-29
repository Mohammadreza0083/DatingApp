using Microsoft.AspNetCore.Identity;
// ReSharper disable NullableWarningSuppressionIsUsed

namespace API.Entities;

public class AppUserRole:IdentityUserRole<int>
{
    public AppUsers Users { get; set; }= null!;
    public AppRole Roles { get; set; }= null!;
}