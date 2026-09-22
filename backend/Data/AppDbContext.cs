using Checklist.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Checklist.Api.Data;

/// <summary>
/// EF Core database context for checklist entities.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Creates the context with standard dependency injection options.
    /// </summary>
    /// <param name="options">Configured options including the SQLite connection.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Users table for future multi-user support.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Todo items table.
    /// </summary>
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    /// <summary>
    /// OAuth credential table containing encrypted token values.
    /// </summary>
    public DbSet<GoogleOAuthCredential> GoogleOAuthCredentials => Set<GoogleOAuthCredential>();

    /// <summary>
    /// Configures table relationships, constraints, and indexes.
    /// </summary>
    /// <param name="modelBuilder">Model builder for EF entity configuration.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(x => x.DisplayName).IsRequired().HasMaxLength(120);
        });

        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(4000);
            entity.HasOne(x => x.User)
                .WithMany(u => u.TodoItems)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.UserId, x.IsDone, x.DueDate });
        });

        modelBuilder.Entity<GoogleOAuthCredential>(entity =>
        {
            entity.HasOne(x => x.User)
                .WithOne(u => u.GoogleOAuthCredential)
                .HasForeignKey<GoogleOAuthCredential>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.UserId).IsUnique();
        });
    }
}
