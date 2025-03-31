using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

public static class IdentityServiceExtension
{
    public static IServiceCollection AddIdentityServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<AppUsers>(op =>
            {
                op.Password.RequireDigit = true;
            })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddEntityFrameworkStores<DataContext>();
        
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
        services.AddAuthorizationBuilder()
            .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
            .AddPolicy("ModeratorPhotoRole", policy => policy.RequireRole("Moderator", "Admin"));
        return services;
    }
}
