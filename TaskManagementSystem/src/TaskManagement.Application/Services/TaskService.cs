namespace TaskManagement.Application.Services;

using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Validators;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Shared.Exceptions;

/// <summary>
/// TaskService implements ITaskService.
/// This is where BUSINESS LOGIC lives.
/// 
/// Architecture Pattern: Service Layer
/// - Handles use cases (create, read, update, delete)
/// - Validates input data
/// - Calls repository for data access
/// - Transforms entities to DTOs
/// - Typical flow: API → Service → Repository → Database
/// </summary>
public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly TaskValidator _taskValidator;

    public TaskService(ITaskRepository taskRepository, TaskValidator taskValidator)
    {
        _taskRepository = taskRepository;
        _taskValidator = taskValidator;
    }

    /// <summary>
    /// Create a new task
    /// Flow: Validate → Create Entity → Save to DB → Return DTO
    /// </summary>
    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto)
    {
        // Step 1: Validate input
        var validationResult = _taskValidator.ValidateCreate(createTaskDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.ErrorMessage);

        // Step 2: Create entity
        var taskItem = new TaskItem
        {
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,
            IsCompleted = false
        };

        // Step 3: Save to database
        await _taskRepository.CreateAsync(taskItem);

        // Step 4: Convert to DTO and return
        return MapToDto(taskItem);
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    public async Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto)
    {
        // Step 1: Validate input
        var validationResult = _taskValidator.ValidateUpdate(updateTaskDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.ErrorMessage);

        // Step 2: Get existing task
        var task = await _taskRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Task with ID {id} not found");

        // Step 3: Update using entity's business logic
        task.Update(updateTaskDto.Title, updateTaskDto.Description);

        // Step 4: Save to database
        await _taskRepository.UpdateAsync(task);

        // Step 5: Return DTO
        return MapToDto(task);
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    public async Task<bool> DeleteTaskAsync(int id)
    {
        var exists = await _taskRepository.GetByIdAsync(id) != null;
        if (!exists)
            throw new NotFoundException($"Task with ID {id} not found");

        return await _taskRepository.DeleteAsync(id);
    }

    /// <summary>
    /// Mark task as completed
    /// Uses the entity's business logic method
    /// </summary>
    public async Task<TaskDto> MarkTaskAsCompletedAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Task with ID {id} not found");

        // Use the entity's business logic method
        task.MarkAsCompleted();

        // Save changes
        await _taskRepository.UpdateAsync(task);

        return MapToDto(task);
    }

    /// <summary>
    /// Get task by ID
    /// </summary>
    public async Task<TaskDto?> GetTaskByIdAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        return task == null ? null : MapToDto(task);
    }

    /// <summary>
    /// Get all tasks
    /// </summary>
    public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
    {
        var tasks = await _taskRepository.GetAllAsync();
        return tasks.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Get only completed tasks
    /// </summary>
    public async Task<IEnumerable<TaskDto>> GetCompletedTasksAsync()
    {
        var tasks = await _taskRepository.GetCompletedAsync();
        return tasks.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Helper: Map Entity to DTO
    /// This ensures API clients never see our internal entity structure
    /// </summary>
    private static TaskDto MapToDto(TaskItem task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}
