namespace TaskManagement.Web.ViewModels;

using TaskManagement.Application.DTOs;

/// <summary>
/// ViewModel for the task list view
/// ViewModels are specifically for the UI layer
/// They contain data needed for rendering views
/// </summary>
public class TaskListViewModel
{
    public List<TaskDto> Tasks { get; set; } = new();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// ViewModel for create/edit views
/// </summary>
public class TaskFormViewModel
{
    public int? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsEditMode => Id.HasValue;
}
