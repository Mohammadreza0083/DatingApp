using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

/// <summary>
/// Extension methods for configuring identity services
/// </summary>
public static class IdentityServiceExtension
{
    /// <summary>
    /// Adds and configures identity services including authentication and authorization
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The service collection with added identity services</returns>
    public static IServiceCollection AddIdentityServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        // Configure identity core with custom user type and password requirements
        services.AddIdentityCore<AppUsers>(op =>
            {
                op.Password.RequireDigit = true;
            })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddEntityFrameworkStores<DataContext>();
        
        // Configure JWT authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(op =>
        {
            string tokenKey = configuration["TokenKey"] ?? throw new InvalidOperationException("TokenKey is missing");
            op.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            // Configure token handling for SignalR hubs
            op.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    string? accessToken = context.Request.Query["access_token"];
                    string path = context.HttpContext.Request.Path;
                    if (path.StartsWith("/hubs") && !string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        // Configure authorization policies
        services.AddAuthorizationBuilder()
            .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
            .AddPolicy("ModeratorPhotoRole", policy => policy.RequireRole("Moderator", "Admin"));
        return services;
    }
}
