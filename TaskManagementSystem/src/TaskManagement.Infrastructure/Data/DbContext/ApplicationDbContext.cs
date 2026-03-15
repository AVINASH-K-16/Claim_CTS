namespace TaskManagement.Infrastructure.Data.DbContext;

using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;

/// <summary>
/// ApplicationDbContext is the database configuration file.
/// 
/// What it does:
/// - Defines the database tables (DbSets)
/// - Configures how entities map to database columns
/// - Manages database migrations
/// - Provides methods to query the database
///
/// This is Entity Framework Core's DbContext - the bridge between C# objects and database tables.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets represent database tables
    public DbSet<TaskItem> Tasks { get; set; }

    /// <summary>
    /// Configure model mappings (Entity Configuration)
    /// This tells EF Core how to map our C# entities to database tables
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure TaskItem table
        modelBuilder.Entity<TaskItem>(entity =>
        {
            // Set the primary key
            entity.HasKey(e => e.Id);

            // Configure properties
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.IsCompleted)
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()"); // SQL Server function for current UTC time

            // Create an index for frequently queried columns
            entity.HasIndex(e => e.IsCompleted)
                .HasName("IX_Tasks_IsCompleted");
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    /// <summary>
    /// Add sample data to the database on creation
    /// </summary>
    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>().HasData(
            new TaskItem
            {
                Id = 1,
                Title = "Learn Clean Architecture",
                Description = "Understand the layers and how they interact",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                Id = 2,
                Title = "Build a Task Management API",
                Description = "Create a REST API using Domain-driven design",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                Id = 3,
                Title = "Write Unit Tests",
                Description = "Test the business logic",
                IsCompleted = true,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            }
        );
    }
}
