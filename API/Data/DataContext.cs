using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : IdentityDbContext<AppUsers, AppRole, 
    int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, 
    IdentityRoleClaim<int>, IdentityUserToken<int> >(options)
{
    public DbSet<UserLike> Likes { get; set; }
    
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
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
        
        modelBuilder.Entity<Message>()
            .HasOne(r => r.Recipient)
            .WithMany(r => r.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Message>()
            .HasOne(s => s.Sender)
            .WithMany(s => s.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 