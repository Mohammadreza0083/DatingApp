using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Class for seeding initial data into the database
/// </summary>
public class Seed
{
    /// <summary>
    /// Seeds the database with initial user data
    /// Creates default roles and users including an admin user
    /// </summary>
    /// <param name="manager">UserManager instance for managing users</param>
    /// <param name="logger">ILogger instance for logging seeding progress</param>
    /// <param name="roleManager">RoleManager instance for managing roles</param>
    /// <returns>A Task representing the asynchronous operation</returns>
    public static async Task SeedUser(UserManager<AppUsers> manager, ILogger logger, RoleManager<AppRole> roleManager)
    {
        if (await manager.Users.AnyAsync())
        {
            return;
        }
        string userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        JsonSerializerOptions options = 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        List<AppUsers>? users = 
            JsonSerializer.Deserialize<List<AppUsers>>(userData, options);
        if (users is null)
        {
            return;
        }
        var roles = new List<AppRole>()
        {
            new() { Name = "Member" },
            new() { Name = "Admin" },
            new() { Name = "Moderator" }
        };
        foreach (var role in roles)
        {
            if (role.Name is not  null && !await roleManager.RoleExistsAsync(role.Name) )
            {
                await roleManager.CreateAsync(role);
            }
        }
        foreach (AppUsers user in users)
        {
            user.Photos.First().IsApproved = true;
            if(user.UserName is not null)
                user.UserName = user.UserName.ToLower();
            logger.LogInformation($"Seeding user {user.UserName}");
            await manager.CreateAsync(user, "Pa$$w0rd");
            await manager.AddToRoleAsync(user, "Member");
        }
        var admin = new AppUsers()
        {
            UserName = "admin",
            KnownAs = "admin",
            Gender = "",
            City = "",
            Country = ""
        };
        await manager.CreateAsync(admin, "Pa$$w0rd");
        await manager.AddToRolesAsync(admin,["Admin", "Moderator"]);
        logger.LogInformation("Seeded users successfully");
    }
}