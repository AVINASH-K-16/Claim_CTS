namespace TaskManagement.Application.Interfaces;

using TaskManagement.Application.DTOs;

/// <summary>
/// ITaskService defines the contract for task operations.
/// 
/// Why an interface?
/// - Defines what the service must do
/// - Can be mocked for testing
/// - Allows multiple implementations
/// - Decouples the API from the business logic
/// </summary>
public interface ITaskService
{
    // Command operations (modify data)
    Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto);
    Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto);
    Task<bool> DeleteTaskAsync(int id);
    Task<TaskDto> MarkTaskAsCompletedAsync(int id);

    // Query operations (read data)
    Task<TaskDto?> GetTaskByIdAsync(int id);
    Task<IEnumerable<TaskDto>> GetAllTasksAsync();
    Task<IEnumerable<TaskDto>> GetCompletedTasksAsync();
}
