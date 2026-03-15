namespace TaskManagement.Shared.Exceptions;

/// <summary>
/// Custom exception for when a resource (like a task) is not found.
/// This is thrown from the Application or Infrastructure layers.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

/// <summary>
/// Custom exception for validation errors.
/// Used when business rules are violated.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

/// <summary>
/// Custom exception for general application errors.
/// </summary>
public class ApplicationException : Exception
{
    public ApplicationException(string message) : base(message) { }
}
