namespace TaskManagement.Domain.Interfaces;

using TaskManagement.Domain.Entities;

/// <summary>
/// ITaskRepository is a contract (interface) that defines what operations
/// are needed to persist tasks to a database.
///
/// Why use an interface?
/// - Allows different implementations (SqlServer, MySql, InMemory)
/// - Enables testing with mock repositories
/// - Decouples business logic from database code
///
/// This is an example of the Repository Pattern.
/// </summary>
public interface ITaskRepository
{
    // READ operations
    Task<TaskItem?> GetByIdAsync(int id);
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<IEnumerable<TaskItem>> GetCompletedAsync();

    // WRITE operations
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem> UpdateAsync(TaskItem task);
    Task<bool> DeleteAsync(int id);

    // UTILITY operations
    Task<int> SaveChangesAsync();
}
