namespace TaskManagement.Domain.Enums;

/// <summary>
/// Represents the status of a task.
/// This enum is part of the Domain Layer because it contains business rules.
/// </summary>
public enum TaskStatus
{
    /// <summary>Task is waiting to be started</summary>
    Todo = 0,

    /// <summary>Task is currently being worked on</summary>
    InProgress = 1,

    /// <summary>Task has been completed</summary>
    Completed = 2,

    /// <summary>Task is on hold</summary>
    OnHold = 3
}
