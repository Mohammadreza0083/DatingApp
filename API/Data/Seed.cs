using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
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