namespace TaskManagement.Application.Validators;

using TaskManagement.Application.DTOs;
using TaskManagement.Shared.Constants;
using TaskManagement.Shared.Helpers;

/// <summary>
/// TaskValidator handles validation of task data.
/// 
/// Why separate validation?
/// - Keeps business logic clean
/// - Reusable validation across services
/// - Easy to modify validation rules
///
/// When: Before creating or updating
/// </summary>
public class TaskValidator
{
    /// <summary>
    /// Validate CreateTaskDto
    /// </summary>
    public ValidationResult ValidateCreate(CreateTaskDto dto)
    {
        // Validate title
        if (!ValidationHelper.IsValidTaskTitle(dto.Title))
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = AppConstants.InvalidTitleMessage
            };

        // Validate description
        if (!ValidationHelper.IsValidTaskDescription(dto.Description))
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "Description exceeds maximum length"
            };

        return new ValidationResult { IsValid = true };
    }

    /// <summary>
    /// Validate UpdateTaskDto
    /// </summary>
    public ValidationResult ValidateUpdate(UpdateTaskDto dto)
    {
        // Validate title
        if (!ValidationHelper.IsValidTaskTitle(dto.Title))
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = AppConstants.InvalidTitleMessage
            };

        // Validate description
        if (!ValidationHelper.IsValidTaskDescription(dto.Description))
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "Description exceeds maximum length"
            };

        return new ValidationResult { IsValid = true };
    }
}

/// <summary>
/// Simple result object for validation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
}
