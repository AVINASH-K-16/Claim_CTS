using Moq;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;
using TaskManagement.Application.Validators;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Shared.Exceptions;
using Xunit;

namespace TaskManagement.UnitTests;

/// <summary>
/// Unit tests for TaskService
/// 
/// What we're testing:
/// - Service logic without database
/// - Using MOCKS to simulate repository
/// - Validation works correctly
///
/// Why unit tests?
/// - Fast (no database)
/// - Isolated (test only service logic)
/// - Easy to understand business requirements
/// </summary>
public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly TaskValidator _validator;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _validator = new TaskValidator();
        _service = new TaskService(_mockRepository.Object, _validator);
    }

    /// <summary>
    /// Test: Creating a valid task should succeed
    /// </summary>
    [Fact]
    public async Task CreateTaskAsync_WithValidData_ShouldCreateTask()
    {
        // Arrange
        var createDto = new CreateTaskDto
        {
            Title = "Learn Clean Architecture",
            Description = "Understand layers and patterns"
        };

        var createdTask = new TaskItem
        {
            Id = 1,
            Title = createDto.Title,
            Description = createDto.Description,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(createdTask);

        // Act
        var result = await _service.CreateTaskAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdTask.Id, result.Id);
        Assert.Equal(createDto.Title, result.Title);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    /// <summary>
    /// Test: Creating a task with invalid title should fail
    /// </summary>
    [Fact]
    public async Task CreateTaskAsync_WithInvalidTitle_ShouldThrowValidationException()
    {
        // Arrange
        var createDto = new CreateTaskDto
        {
            Title = "AB", // Too short (minimum 3 characters)
            Description = "Description"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _service.CreateTaskAsync(createDto));

        Assert.Contains("Title", exception.Message);
    }

    /// <summary>
    /// Test: Updating a non-existent task should fail
    /// </summary>
    [Fact]
    public async Task UpdateTaskAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var updateDto = new UpdateTaskDto
        {
            Title = "Updated Title",
            Description = "Updated Description"
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((TaskItem)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UpdateTaskAsync(999, updateDto));

        Assert.Contains("not found", exception.Message);
    }

    /// <summary>
    /// Test: Getting all tasks should return list
    /// </summary>
    [Fact]
    public async Task GetAllTasksAsync_ShouldReturnAllTasks()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = 1, Title = "Task 1", IsCompleted = false, CreatedAt = DateTime.UtcNow },
            new TaskItem { Id = 2, Title = "Task 2", IsCompleted = true, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(tasks);

        // Act
        var result = await _service.GetAllTasksAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    /// <summary>
    /// Test: Getting task by ID should return task
    /// </summary>
    [Fact]
    public async Task GetTaskByIdAsync_WithValidId_ShouldReturnTask()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "Test Task",
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(task);

        // Act
        var result = await _service.GetTaskByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(task.Id, result.Id);
        Assert.Equal(task.Title, result.Title);
    }

    /// <summary>
    /// Test: Marking task as completed should update IsCompleted
    /// </summary>
    [Fact]
    public async Task MarkTaskAsCompletedAsync_ShouldMarkTaskAsCompleted()
    {
        // Arrange
        var task = new TaskItem
        {
            Id = 1,
            Title = "Task to Complete",
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(task);

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(task);

        // Act
        var result = await _service.MarkTaskAsCompletedAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsCompleted);
    }

    /// <summary>
    /// Test: Deleting a non-existent task should fail
    /// </summary>
    [Fact]
    public async Task DeleteTaskAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((TaskItem)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DeleteTaskAsync(999));

        Assert.Contains("not found", exception.Message);
    }

    /// <summary>
    /// Test: Deleting an existing task should succeed
    /// </summary>
    [Fact]
    public async Task DeleteTaskAsync_WithValidId_ShouldDeleteTask()
    {
        // Arrange
        var task = new TaskItem { Id = 1, Title = "Task to Delete", CreatedAt = DateTime.UtcNow };

        _mockRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(task);

        _mockRepository
            .Setup(r => r.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteTaskAsync(1);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}
