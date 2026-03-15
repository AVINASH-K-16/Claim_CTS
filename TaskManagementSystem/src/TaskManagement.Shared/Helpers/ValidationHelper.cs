namespace TaskManagement.Shared.Helpers;

/// <summary>
/// Validation helper class for common validations
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validate task title length
    /// </summary>
    public static bool IsValidTaskTitle(string title)
    {
        return !string.IsNullOrWhiteSpace(title) &&
               title.Length >= Constants.AppConstants.TaskTitleMinLength &&
               title.Length <= Constants.AppConstants.TaskTitleMaxLength;
    }

    /// <summary>
    /// Validate task description length
    /// </summary>
    public static bool IsValidTaskDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return true; // Description is optional

        return description.Length <= Constants.AppConstants.TaskDescriptionMaxLength;
    }
}
