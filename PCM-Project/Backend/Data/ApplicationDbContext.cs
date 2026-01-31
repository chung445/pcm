using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PCM.API.Models;

namespace PCM.API.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Member> Members { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<TransactionCategory> TransactionCategories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Court> Courts { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Challenge> Challenges { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<MatchScore> MatchScores { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships to prevent cascade delete issues
        modelBuilder.Entity<Match>()
            .HasOne(m => m.Team1_Player1)
            .WithMany()
            .HasForeignKey(m => m.Team1_Player1Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Team1_Player2)
            .WithMany()
            .HasForeignKey(m => m.Team1_Player2Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Team2_Player1)
            .WithMany()
            .HasForeignKey(m => m.Team2_Player1Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.Team2_Player2)
            .WithMany()
            .HasForeignKey(m => m.Team2_Player2Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Creator)
            .WithMany(m => m.Transactions)
            .HasForeignKey(t => t.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Challenge>()
            .HasOne(c => c.Creator)
            .WithMany(m => m.CreatedChallenges)
            .HasForeignKey(c => c.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure enum conversions
        modelBuilder.Entity<Member>()
            .Property(m => m.Status)
            .HasConversion<string>();

        modelBuilder.Entity<TransactionCategory>()
            .Property(tc => tc.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Booking>()
            .Property(b => b.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Challenge>()
            .Property(c => c.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Challenge>()
            .Property(c => c.GameMode)
            .HasConversion<string>();

        modelBuilder.Entity<Challenge>()
            .Property(c => c.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Match>()
            .Property(m => m.MatchFormat)
            .HasConversion<string>();

        modelBuilder.Entity<Match>()
            .Property(m => m.WinningSide)
            .HasConversion<string>();

        modelBuilder.Entity<Participant>()
            .Property(p => p.Team)
            .HasConversion<string>();

        modelBuilder.Entity<Participant>()
            .Property(p => p.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Notification>()
            .Property(n => n.Type)
            .HasConversion<string>();
    }
}
