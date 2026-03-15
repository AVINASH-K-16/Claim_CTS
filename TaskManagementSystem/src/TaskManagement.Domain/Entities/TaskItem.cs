namespace TaskManagement.Domain.Entities;

/// <summary>
/// TaskItem is the core business entity in our Domain Layer.
/// 
/// Key Concepts:
/// - It represents a Todo item or Task
/// - Contains ONLY business data (no database-specific logic)
/// - Can be used in any layer without database dependencies
/// - Rules: Title is required, dates are important for tracking
/// </summary>
public class TaskItem
{
    // Primary key
    public int Id { get; set; }

    // Core properties
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }

    // Audit properties (important for tracking)
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Constructor for creating new tasks
    public TaskItem()
    {
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Business logic: Mark task as completed
    /// This method ensures business rules are enforced
    /// </summary>
    public void MarkAsCompleted()
    {
        IsCompleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Business logic: Update task
    /// This method ensures business rules are enforced
    /// </summary>
    public void Update(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty");

        Title = title;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
}
