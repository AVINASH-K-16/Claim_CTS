namespace TaskManagement.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Shared.Exceptions;

/// <summary>
/// TasksController handles HTTP requests for task operations.
/// 
/// REST Conventions:
/// GET    /api/tasks        → Get all tasks
/// GET    /api/tasks/{id}   → Get task by ID
/// POST   /api/tasks        → Create new task
/// PUT    /api/tasks/{id}   → Update task
/// DELETE /api/tasks/{id}   → Delete task
///
/// Data Flow: 
/// 1. Request arrives at controller
/// 2. Controller calls service
/// 3. Service validates and uses repository
/// 4. Repository gets/saves data
/// 5. Data flows back through layers
/// 6. Controller returns HTTP response
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    // Dependency Injection: ITaskService is injected by ASP.NET Core
    // The framework looks at ITaskService in Program.cs and provides the implementation
    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// GET: /api/tasks
    /// Get all tasks
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTasks()
    {
        try
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error retrieving tasks", error = ex.Message });
        }
    }

    /// <summary>
    /// GET: /api/tasks/{id}
    /// Get task by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskById(int id)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound(new { message = $"Task with ID {id} not found" });

            return Ok(task);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error retrieving task", error = ex.Message });
        }
    }

    /// <summary>
    /// POST: /api/tasks
    /// Create a new task
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var createdTask = await _taskService.CreateTaskAsync(createTaskDto);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error creating task", error = ex.Message });
        }
    }

    /// <summary>
    /// PUT: /api/tasks/{id}
    /// Update an existing task
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedTask = await _taskService.UpdateTaskAsync(id, updateTaskDto);
            return Ok(updatedTask);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error updating task", error = ex.Message });
        }
    }

    /// <summary>
    /// DELETE: /api/tasks/{id}
    /// Delete a task
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            var deleted = await _taskService.DeleteTaskAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Task with ID {id} not found" });

            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error deleting task", error = ex.Message });
        }
    }

    /// <summary>
    /// POST: /api/tasks/{id}/complete
    /// Mark a task as completed
    /// </summary>
    [HttpPost("{id}/complete")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkTaskAsCompleted(int id)
    {
        try
        {
            var completedTask = await _taskService.MarkTaskAsCompletedAsync(id);
            return Ok(completedTask);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error completing task", error = ex.Message });
        }
    }

    /// <summary>
    /// GET: /api/tasks/completed
    /// Get only completed tasks
    /// </summary>
    [HttpGet("completed")]
    [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompletedTasks()
    {
        try
        {
            var tasks = await _taskService.GetCompletedTasksAsync();
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error retrieving completed tasks", error = ex.Message });
        }
    }
}
