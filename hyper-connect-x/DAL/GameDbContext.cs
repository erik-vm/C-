using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public sealed class GameDbContext : DbContext
{
    public DbSet<GameStateEntity> GameStates => Set<GameStateEntity>();
    public DbSet<BoardCellEntity> BoardCells => Set<BoardCellEntity>();
    public DbSet<PlayerProfileEntity> PlayerProfiles => Set<PlayerProfileEntity>();

    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public GameDbContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "hyperconnectx.db");
            var fullPath = Path.GetFullPath(dbPath);
            optionsBuilder.UseSqlite($"Data Source={fullPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameStateEntity>()
            .HasKey(g => g.GameId);

        modelBuilder.Entity<GameStateEntity>()
            .HasMany(g => g.BoardCells)
            .WithOne(c => c.GameState)
            .HasForeignKey(c => c.GameStateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BoardCellEntity>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<PlayerProfileEntity>()
            .HasKey(p => p.PlayerName);
    }
}