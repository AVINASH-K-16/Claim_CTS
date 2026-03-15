namespace TaskManagement.Shared.Constants;

/// <summary>
/// Application-wide constants
/// </summary>
public static class AppConstants
{
    // Validation Rules
    public const int TaskTitleMinLength = 3;
    public const int TaskTitleMaxLength = 200;
    public const int TaskDescriptionMaxLength = 1000;

    // Error Messages
    public const string TaskNotFoundMessage = "Task with ID {0} not found";
    public const string InvalidTitleMessage = "Task title must be between 3 and 200 characters";
    public const string TaskAlreadyCompletedMessage = "Task is already completed";

    // Success Messages
    public const string TaskCreatedSuccessfully = "Task created successfully";
    public const string TaskUpdatedSuccessfully = "Task updated successfully";
    public const string TaskDeletedSuccessfully = "Task deleted successfully";
}
