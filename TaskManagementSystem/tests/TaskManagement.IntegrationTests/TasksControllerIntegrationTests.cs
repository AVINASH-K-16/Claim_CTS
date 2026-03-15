using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskManagement.API;
using TaskManagement.Application.DTOs;
using Xunit;

namespace TaskManagement.IntegrationTests;

/// <summary>
/// Integration tests for the API
/// 
/// What we're testing:
/// - Full request → response cycle
/// - Database integration
/// - HTTP status codes
/// - Real data persistence
///
/// Why integration tests?
/// - Test complete flow
/// - Catch issues between layers
/// - Test with actual database
/// </summary>
public class TasksControllerIntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Test: GET /api/tasks should return all tasks
    /// </summary>
    [Fact]
    public async Task GetAllTasks_ShouldReturnAllTasks()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var tasks = await response.Content.ReadAsAsync<IEnumerable<TaskDto>>();
        Assert.NotNull(tasks);
        Assert.NotEmpty(tasks);
    }

    /// <summary>
    /// Test: POST /api/tasks should create a new task
    /// </summary>
    [Fact]
    public async Task CreateTask_WithValidData_ShouldCreateTask()
    {
        // Arrange
        var createDto = new CreateTaskDto
        {
            Title = "Integration Test Task",
            Description = "This is a test task"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdTask = await response.Content.ReadAsAsync<TaskDto>();
        Assert.NotNull(createdTask);
        Assert.Equal(createDto.Title, createdTask.Title);
        Assert.False(createdTask.IsCompleted);
    }

    /// <summary>
    /// Test: POST /api/tasks with invalid title should fail
    /// </summary>
    [Fact]
    public async Task CreateTask_WithInvalidTitle_ShouldReturnBadRequest()
    {
        // Arrange
        var createDto = new CreateTaskDto
        {
            Title = "AB", // Too short
            Description = "Description"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Test: GET /api/tasks/{id} should return specific task
    /// </summary>
    [Fact]
    public async Task GetTaskById_WithValidId_ShouldReturnTask()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var task = await response.Content.ReadAsAsync<TaskDto>();
        Assert.NotNull(task);
        Assert.Equal(1, task.Id);
    }

    /// <summary>
    /// Test: GET /api/tasks/{id} with invalid ID should return 404
    /// </summary>
    [Fact]
    public async Task GetTaskById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// Test: PUT /api/tasks/{id} should update task
    /// </summary>
    [Fact]
    public async Task UpdateTask_WithValidData_ShouldUpdateTask()
    {
        // Arrange
        var updateDto = new UpdateTaskDto
        {
            Title = "Updated Task Title",
            Description = "Updated Description"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/tasks/1", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedTask = await response.Content.ReadAsAsync<TaskDto>();
        Assert.NotNull(updatedTask);
        Assert.Equal("Updated Task Title", updatedTask.Title);
    }

    /// <summary>
    /// Test: DELETE /api/tasks/{id} should delete task
    /// </summary>
    [Fact]
    public async Task DeleteTask_WithValidId_ShouldDeleteTask()
    {
        // First, create a task to delete
        var createDto = new CreateTaskDto
        {
            Title = "Task to Delete",
            Description = "Delete me"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/tasks", createDto);
        var createdTask = await createResponse.Content.ReadAsAsync<TaskDto>();
        
        // Act
        var response = await _client.DeleteAsync($"/api/tasks/{createdTask!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify task is deleted
        var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    /// <summary>
    /// Test: POST /api/tasks/{id}/complete should mark task as completed
    /// </summary>
    [Fact]
    public async Task CompleteTask_WithValidId_ShouldMarkAsCompleted()
    {
        // Act
        var response = await _client.PostAsync("/api/tasks/1/complete", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var completedTask = await response.Content.ReadAsAsync<TaskDto>();
        Assert.NotNull(completedTask);
        Assert.True(completedTask.IsCompleted);
    }

    /// <summary>
    /// Test: GET /api/tasks/completed should return only completed tasks
    /// </summary>
    [Fact]
    public async Task GetCompletedTasks_ShouldReturnOnlyCompletedTasks()
    {
        // First complete a task
        var completeResponse = await _client.PostAsync("/api/tasks/1/complete", null);
        Assert.Equal(HttpStatusCode.OK, completeResponse.StatusCode);

        // Act
        var response = await _client.GetAsync("/api/tasks/completed");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tasks = await response.Content.ReadAsAsync<IEnumerable<TaskDto>>();
        Assert.NotNull(tasks);
        Assert.All(tasks, task => Assert.True(task.IsCompleted));
    }
}
