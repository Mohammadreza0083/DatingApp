using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Database context for the application
/// Manages database connections and entity configurations
/// </summary>
public class DataContext(DbContextOptions options) : IdentityDbContext<AppUsers, AppRole, 
    int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, 
    IdentityRoleClaim<int>, IdentityUserToken<int> >(options)
{
    /// <summary>
    /// Database set for user likes
    /// </summary>
    public DbSet<UserLike> Likes { get; set; }

    /// <summary>
    /// Database set for messages
    /// </summary>
    public DbSet<Message> Messages { get; set; }

    /// <summary>
    /// Database set for chat groups
    /// </summary>
    public DbSet<Group> Groups { get; set; }

    /// <summary>
    /// Database set for SignalR connections
    /// </summary>
    public DbSet<Connection> Connections { get; set; }

    /// <summary>
    /// Database set for user photos
    /// </summary>
    public DbSet<Photo> Photos { get; set; }

    /// <summary>
    /// Configures the database model and relationships
    /// </summary>
    /// <param name="modelBuilder">Builder for creating the model</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure user-role relationships
        modelBuilder.Entity<AppUsers>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Users)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();
        
        modelBuilder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Roles)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();
        
        // Configure user likes relationships
        modelBuilder.Entity<UserLike>()
            .HasKey(k=> new {k.SourceUserId, k.TargetUserId});
        
        modelBuilder.Entity<UserLike>()
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<UserLike>()
            .HasOne(t => t.TargetUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(t => t.TargetUserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        // Configure message relationships
        modelBuilder.Entity<Message>()
            .HasOne(r => r.Recipient)
            .WithMany(r => r.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Message>()
            .HasOne(s => s.Sender)
            .WithMany(s => s.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure photo query filter
        modelBuilder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);
    }
} 