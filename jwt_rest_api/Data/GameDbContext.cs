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
    public DbSet<RequestLog> RequestLogs { get; set; } = null!;
    public DbSet<NpcInteraction> NpcInteractions { get; set; } = null!;
    
    // Game Session Architecture
    public DbSet<GameSession> GameSessions { get; set; } = null!;
    public DbSet<SessionEvent> SessionEvents { get; set; } = null!;
    public DbSet<EventEncounter> EventEncounters { get; set; } = null!;
    public DbSet<PlayerResponse> PlayerResponses { get; set; } = null!;
    public DbSet<SessionMinigame> SessionMinigames { get; set; } = null!;
    public DbSet<EventData> EventDataMaster { get; set; } = null!;
    public DbSet<SubEventData> SubEventDataMaster { get; set; } = null!;

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
            
            // Map JSON properties to PostgreSQL jsonb type
            entity.Property(gp => gp.InventoryJson).HasColumnType("jsonb");
            entity.Property(gp => gp.StateDataJson).HasColumnType("jsonb");

            // Define 1:1 relationship with User
            entity.HasOne(gp => gp.User)
                  .WithOne()
                  .HasForeignKey<GameProgress>(gp => gp.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure NpcInteraction
        modelBuilder.Entity<NpcInteraction>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.HasOne(n => n.User)
                  .WithMany(u => u.NpcInteractions)
                  .HasForeignKey(n => n.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure GameSession Hierarchy
        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasOne(s => s.User)
                  .WithMany(u => u.GameSessions)
                  .HasForeignKey(s => s.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SessionEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Session)
                  .WithMany(s => s.Events)
                  .HasForeignKey(e => e.SessionId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.EventData)
                  .WithMany()
                  .HasForeignKey(e => e.EventDataId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EventEncounter>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.SessionEvent)
                  .WithMany(se => se.Encounters)
                  .HasForeignKey(e => e.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PlayerResponse>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.HasOne(r => r.Encounter)
                  .WithMany(e => e.PlayerResponses)
                  .HasForeignKey(r => r.EncounterId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SessionMinigame>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasOne(m => m.SessionEvent)
                  .WithMany(se => se.Minigames)
                  .HasForeignKey(m => m.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EventData>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<SubEventData>(entity =>
        {
            entity.HasKey(s => s.EncounterId);
            entity.HasOne(s => s.EventData)
                  .WithMany(e => e.SubEvents)
                  .HasForeignKey(s => s.EventId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
