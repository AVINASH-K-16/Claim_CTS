namespace TaskManagement.Application.DTOs;

/// <summary>
/// TaskDto is a Data Transfer Object (DTO).
/// 
/// What it does:
/// - Represents task data sent to/from the API
/// - Only includes fields needed by clients
/// - Hides internal implementation details
/// - Protects the entity from direct modification
///
/// Key difference from TaskItem (Entity):
/// - Entity: Lives in database, has business logic
/// - DTO: Transferred over network/API, just data
///
/// When to use:
/// - API responses: return TaskDto (not TaskItem)
/// - API requests: accept CreateTaskDto (not TaskItem)
/// </summary>
public class TaskDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new task
/// Only includes fields the user should provide
/// </summary>
public class CreateTaskDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO for updating a task
/// </summary>
public class UpdateTaskDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
}
