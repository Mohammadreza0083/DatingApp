using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUser(UserManager<AppUsers> manager, ILogger logger)
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

        foreach (AppUsers user in users)
        {
            logger.LogInformation($"Seeding user {user.UserName}");
            await manager.CreateAsync(user, "Pa$$w0rd");
        }
        logger.LogInformation("Seeded users successfully");
    }
}