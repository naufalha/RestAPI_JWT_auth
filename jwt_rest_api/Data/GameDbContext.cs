using Microsoft.EntityFrameworkCore;
using jwt_rest_api.Models;

namespace jwt_rest_api.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<GameProgress> GameProgresses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.GoogleSubjectId).IsRequired().HasMaxLength(255);
            entity.HasIndex(u => u.GoogleSubjectId).IsUnique();
            entity.Property(u => u.Name).HasMaxLength(255);
        });

        // Configure GameProgress
        modelBuilder.Entity<GameProgress>(entity =>
        {
            entity.HasKey(gp => gp.UserId);
            
            // Map JSON properties to MySQL json type (or text/longtext fallback)
            entity.Property(gp => gp.InventoryJson).HasColumnType("json");
            entity.Property(gp => gp.StateDataJson).HasColumnType("json");

            // Define 1:1 relationship with User
            entity.HasOne(gp => gp.User)
                  .WithOne()
                  .HasForeignKey<GameProgress>(gp => gp.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
