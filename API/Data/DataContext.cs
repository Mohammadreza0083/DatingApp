using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUsers> Users { get; set; }
    
    public DbSet<UserLike> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
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
    }
} 