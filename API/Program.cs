using API.Data;
using API.Entities;
using API.Extensions;
using API.Middleware;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Main entry point for the application
var builder = WebApplication.CreateBuilder(args);

// Configure application services
// Add custom application services (database, authentication, etc.)
builder.Services.AddApplicationServices(builder.Configuration);

// Configure identity services (user management, roles, etc.)
builder.Services.AddIdentityServices(builder.Configuration);

// Configure OpenAPI/Swagger for API documentation
builder.Services.AddOpenApi();

// Set the application URL for HTTPS
builder.WebHost.UseUrls("https://localhost:5001");

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
// Add global exception handling middleware
app.UseMiddleware<ExceptionMiddleware>();

// Configure CORS policy
// Allow requests from the Angular development server
app.UseCors(x =>
    x.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .WithOrigins("http://localhost:4200", "https://localhost:4200")
);

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Configure OpenAPI/Swagger UI in development environment
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map controllers and SignalR hubs
app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

// Database migration and seeding
using IServiceScope scope = app.Services.CreateScope();
IServiceProvider services = scope.ServiceProvider;
try
{
    // Get required services
    DataContext context = services.GetRequiredService<DataContext>();
    UserManager<AppUsers> manager = services.GetRequiredService<UserManager<AppUsers>>();
    RoleManager<AppRole> roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    
    // Apply pending migrations
    await context.Database.MigrateAsync();
    
    // Clear existing connections
    await context.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]");
    
    // Seed initial data
    await Seed.SeedUser(manager, new LoggerFactory().CreateLogger<Program>(), roleManager);
}
catch (Exception ex)
{
    // Log any errors during migration/seeding
    ILogger logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex.Message, "An error occurred while migrating the database.");
}

// Start the application
await app.RunAsync();
