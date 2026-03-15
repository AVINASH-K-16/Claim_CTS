namespace TaskManagement.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Data.DbContext;

/// <summary>
/// TaskRepository implements the ITaskRepository interface.
/// 
/// Why a Repository?
/// - Centralizes all database queries for tasks
/// - If database changes, only this file changes
/// - Can mock this for testing
/// - Keeps infrastructure code out of application layer
///
/// Design Pattern: Repository Pattern
/// This provides an abstraction for data access.
/// </summary>
public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get task by ID
    /// Returns null if not found (handled by caller)
    /// </summary>
    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    /// <summary>
    /// Get all tasks
    /// </summary>
    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _context.Tasks
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get only completed tasks
    /// Example of a filtered query
    /// </summary>
    public async Task<IEnumerable<TaskItem>> GetCompletedAsync()
    {
        return await _context.Tasks
            .Where(t => t.IsCompleted)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        // Add to DbContext (in-memory)
        _context.Tasks.Add(task);

        // Save to database
        await _context.SaveChangesAsync();

        return task;
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    public async Task<TaskItem> UpdateAsync(TaskItem task)
    {
        // Mark as modified and save
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();

        return task;
    }

    /// <summary>
    /// Delete a task by ID
    /// Returns true if deleted, false if not found
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var task = await GetByIdAsync(id);
        if (task == null)
            return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Save any pending changes
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
