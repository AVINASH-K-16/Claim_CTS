# Clean Architecture in Enterprise ASP.NET

## Table of Contents
1. [Architecture Overview](#architecture-overview)
2. [Layer by Layer](#layer-by-layer)
3. [Design Patterns](#design-patterns)
4. [Data Flow](#data-flow)
5. [Why Clean Architecture?](#why-clean-architecture)
6. [Best Practices](#best-practices)

---

## Architecture Overview

### What is Clean Architecture?

Clean Architecture is a **design pattern** that emphasizes:

1. **Separation of Concerns** - Each layer has one responsibility
2. **Dependency Inversion** - High-level modules don't depend on low-level modules
3. **Testability** - Business logic is independent and testable
4. **Maintainability** - Easy to modify without breaking other parts
5. **Flexibility** - Swap implementations (databases, frameworks) without changing business logic

### The Concentric Rings

```
┌─────────────────────────────────────────────┐
│  Frameworks & Drivers (UI, Web, DB)        │
├─────────────────────────────────────────────┤
│  Interface Adapters (Controllers, Gateways) │
├─────────────────────────────────────────────┤
│  Application Business Rules (Use Cases)     │
├─────────────────────────────────────────────┤
│  Enterprise Business Rules (Entities)       │
└─────────────────────────────────────────────┘
```

**Key Rule:** Dependencies flow INWARD. Outer layers depend on inner layers, never the opposite.

---

## Layer by Layer

### 1. Domain Layer (Innermost - Independent)

**Responsibility:** Contains pure business logic and entities

**Contents:**
- Entities (e.g., `TaskItem`)
- Value Objects
- Enums (e.g., `TaskStatus`)
- Domain Interfaces

**Characteristics:**
- ✅ **0 dependencies** on other projects
- ✅ Can be used in any application (console, web, mobile)
- ✅ Pure business rules, no framework code
- ✅ Easy to unit test

**Example:**
```csharp
// Domain/Entities/TaskItem.cs
public class TaskItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool IsCompleted { get; set; }
    
    // Business logic method
    public void MarkAsCompleted()
    {
        if (IsCompleted)
            throw new InvalidOperationException("Task already completed");
        IsCompleted = true;
    }
}
```

**Why Separate?**
- Business logic is independent of how data is stored
- Can test business rules without mocking databases
- Can reuse in multiple applications

---

### 2. Shared Layer (Cross-Cutting Concerns)

**Responsibility:** Utilities and helpers used across all layers

**Contents:**
- Custom Exceptions
- Constants
- Extension Methods
- Helper Utilities

**Characteristics:**
- ✅ **0 dependencies** on other layers
- ✅ Used everywhere
- ✅ Global configuration in one place
- ✅ Easy to modify business rules

**Example:**
```csharp
// Shared/Constants/AppConstants.cs
public static class AppConstants
{
    public const int TaskTitleMinLength = 3;
    public const int TaskTitleMaxLength = 200;
}

// Shared/Exceptions/CustomExceptions.cs
public class NotFoundException : Exception { }
public class ValidationException : Exception { }
```

**Why Separate?**
- Change validation rules once, affects all layers
- Standard exceptions across application
- Clear contract for error handling

---

### 3. Application Layer (Business Logic Orchestration)

**Responsibility:** Orchestrates business logic workflows

**Contents:**
- Services (e.g., `TaskService`)
- Service Interfaces (e.g., `ITaskService`)
- Data Transfer Objects (DTOs)
- Validators
- Commands & Queries

**Characteristics:**
- ✅ Depends on Domain + Shared (inward)
- ✅ NO dependencies on Infrastructure or Presentation
- ✅ Business logic is testable without database
- ✅ DTOs protect entities from external exposure

**Data Transfer Objects (DTOs) Explained:**
```csharp
// Domain/Entities/TaskItem.cs (Internal)
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string InternalNotes { get; set; }  // Never exposed!
    public DateTime CreatedAt { get; set; }
}

// Application/DTOs/TaskDto.cs (External)
public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    // InternalNotes is NOT exposed!
    public DateTime CreatedAt { get; set; }
}
```

**Why DTOs?**
- Security: Don't expose sensitive internal fields
- Flexibility: External client doesn't need to know internal structure
- Decoupling: Can change entity without breaking API
- Transformation: Format data for external consumption

**Example Service:**
```csharp
// Application/Services/TaskService.cs
public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    
    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto)
    {
        // 1. Validate
        if (dto.Title.Length < 3)
            throw new ValidationException("Title too short");
        
        // 2. Create entity
        var task = new TaskItem { Title = dto.Title };
        
        // 3. Persist
        await _repository.CreateAsync(task);
        
        // 4. Return DTO
        return new TaskDto { Id = task.Id, Title = task.Title };
    }
}
```

**Why Separate Layer?**
- Business logic isolated from database code
- Easy to test with mocked repository
- Reusable across API and Web presentations
- Single place for validation rules

---

### 4. Infrastructure Layer (Data Access & External Services)

**Responsibility:** Implements interfaces defined in Domain and Application

**Contents:**
- Entity Framework DbContext
- Repositories (implementing `ITaskRepository`)
- Entity Configurations
- Database Migrations
- External Service Integrations

**Characteristics:**
- ✅ Depends on Domain + Shared + Application (inward)
- ✅ Contains ALL database code
- ✅ Easy to swap implementations
- ✅ Can use any ORM or database

**Example Repository:**
```csharp
// Infrastructure/Repositories/TaskRepository.cs
public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
    }
    
    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }
}
```

**Repository Pattern:**
```
Application Layer → ITaskRepository Interface
                         ↑
                    (depends on)
                         ↑
Infrastructure Layer → TaskRepository Implementation
```

**Why This Pattern?**
- Application doesn't know about database
- Can mock repository for testing
- Can swap implementations (SQL Server → PostgreSQL)
- SOLID principle: Dependency Inversion

**Changing Database:**
```csharp
// Old: SQL Server
options.UseSqlServer("Server=...;Database=TaskDb")

// New: PostgreSQL
options.UseNpgsql("Host=...;Database=TaskDb")

// All other layers unchanged! ✅
```

---

### 5. API Layer (REST Interface)

**Responsibility:** Exposes application features via REST endpoints

**Contents:**
- Controllers (ASP.NET Core)
- DTOs (API contracts)
- Middleware
- Program.cs (Dependency Injection setup)

**Characteristics:**
- ✅ Depends on Application + Shared (inward)
- ✅ Stateless HTTP requests
- ✅ DTOs for request/response contracts
- ✅ HTTP status codes and error handling

**Example Controller:**
```csharp
// API/Controllers/TasksController.cs
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;
    
    public TasksController(ITaskService service)
    {
        _service = service;  // Injected!
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
    {
        try
        {
            var result = await _service.CreateTaskAsync(dto);
            return CreatedAtAction(nameof(GetTaskById), 
                new { id = result.Id }, result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
```

**REST Best Practices:**
- Use proper HTTP methods (GET, POST, PUT, DELETE)
- Return appropriate status codes (200, 201, 400, 404)
- DTOs for contracts
- Error handling at this layer

---

### 6. Web Layer (HTML Interface)

**Responsibility:** Exposes application features via Razor pages

**Contents:**
- MVC Controllers
- Razor Views (.cshtml)
- ViewModels
- Forms and HTML

**Characteristics:**
- ✅ Depends on Application + Shared (same dependencies as API)
- ✅ Uses same service layer
- ✅ ViewModels for UI data
- ✅ Session and cookie support

**Key Difference from API:**
```
API Layer          → Returns JSON → JavaScript/Mobile Apps
                                  → Postman/REST clients

Web Layer         → Returns HTML → Web Browsers
```

**Both use the same Application Business Logic!**

---

## Design Patterns

### 1. Repository Pattern

**Problem:** Application shouldn't know about database code

**Solution:**
```csharp
// Domain defines interface
public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(int id);
    Task<TaskItem> CreateAsync(TaskItem task);
}

// Infrastructure implements
public class TaskRepository : ITaskRepository { }

// Application uses interface (doesn't know about DbContext)
public class TaskService
{
    private readonly ITaskRepository _repo;
    // Only knows about ITaskRepository, not implementation!
}
```

**Benefit:**
- Swap database without changing service
- Easy to test with mock repository
- Database code isolated

### 2. Dependency Injection

**Problem:** Classes creating their own dependencies → tight coupling

**Solution:**
```csharp
// In Program.cs
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<TaskValidator>();

// In Controller
public class TasksController
{
    public TasksController(ITaskService service)
    {
        _service = service;  // Injected by framework!
    }
}
```

**Benefit:**
- Loose coupling
- Easy to mock
- Framework manages lifetime

### 3. Service Layer

**Problem:** Business logic mixed with presentation code

**Solution:**
```
Controller → Service (business logic) → Repository (data access)
 (HTTP)         (domain logic)          (database)
```

**Benefit:**
- Business logic isolated
- Reusable across presentations
- Easy to test

### 4. Data Transfer Objects (DTOs)

**Problem:** Exposing internal entities breaks encapsulation

**Solution:**
```
Entity (internal) → Convert in Service → DTO (external)
```

**Benefit:**
- Hide internal fields
- Security
- Flexibility to change entity

---

## Data Flow

### Example: Create Task Request

```
1. HTTP Request arrives
   POST /api/tasks
   { "title": "Learn Clean Architecture" }
   
2. TasksController.CreateTask()
   - Routes request to controller
   
3. Input Validation
   - Check ModelState
   
4. TaskService.CreateTaskAsync()
   - Validate business rules
   - Create TaskItem entity
   
5. TaskRepository.CreateAsync()
   - Add to DbContext
   - SaveChangesAsync()
   
6. Database
   - INSERT into Tasks table
   
7. Response Journey (back up)
   - Convert Entity to DTO
   - TaskDto { Id = 1, Title = "..." }
   
8. HTTP Response
   201 Created
   Location: /api/tasks/1
   { "id": 1, "title": "Learn Clean Architecture" }
```

### Request Path & Responsibilities

```
                    REQUEST
                      ↓
    ┌─────────────────────────────────┐
    │  API LAYER (Controllers)        │
    │  - Route request                │
    │  - Parse JSON to DTO            │
    │  - Call service                 │
    │  - Return HTTP response         │
    └─────────────────────────────────┘
                      ↓
    ┌─────────────────────────────────┐
    │  APPLICATION LAYER              │
    │  - Validate input               │
    │  - Enforce business rules       │
    │  - Orchestrate workflow         │
    │  - Convert Entity ↔ DTO         │
    └─────────────────────────────────┘
                      ↓
    ┌─────────────────────────────────┐
    │  INFRASTRUCTURE LAYER           │
    │  - Execute queries              │
    │  - Map objects to DB rows       │
    │  - Handle transactions          │
    └─────────────────────────────────┘
                      ↓
    ┌─────────────────────────────────┐
    │  DATABASE                       │
    │  - Execute SQL                  │
    │  - Store/retrieve data          │
    └─────────────────────────────────┘
                      ↓
                   RESPONSE
```

---

## Why Clean Architecture?

### Problem: Monolithic Code
```csharp
// ❌ Bad: Business logic mixed with database code
public ActionResult CreateTask(string title)
{
    using (var db = new SqlConnection("..."))
    {
        db.Open();
        var cmd = db.CreateCommand();
        cmd.CommandText = "INSERT INTO Tasks (Title) VALUES (@title)";
        cmd.Parameters.AddWithValue("@title", title);
        
        return Ok(new { id = 1, title });
    }
}
```

**Problems:**
- Hard to test (need real database)
- Hard to change (business logic + SQL mixed)
- Hard to reuse (specific to ASP.NET MVC)
- Tight coupling

### Solution: Clean Architecture
```csharp
// ✅ Good: Separated concerns
public class TasksController
{
    private readonly ITaskService _service;
    
    public IActionResult CreateTask(CreateTaskDto dto)
    {
        var task = await _service.CreateTaskAsync(dto);
        return Ok(task);
    }
}
```

**Benefits:**
- Easy to test (mock service)
- Clean and maintainable
- Reusable (API and Web)
- Loose coupling
- Business logic independent

---

## Best Practices

### 1. Dependency Direction

**Rule:** Dependencies flow INWARD only

```
Valid:
API → Application
Application → Domain
Infrastructure → Application
Shared → (no dependencies)

Invalid:
Domain → Application  ❌
Application → API     ❌
```

### 2. Naming Conventions

```
Interfaces:           I{EntityName}Service, I{EntityName}Repository
Services:             {EntityName}Service, {EntityName}Manager
Repositories:         {EntityName}Repository
DTOs:                 {EntityName}Dto, Create{EntityName}Dto
Controllers:          {Entity}sController (plural)
Validators:           {EntityName}Validator
```

### 3. Validation Layers

```
1. Model Validation (API Layer)
   - Check DTO has required fields
   
2. Business Validation (Application Layer)
   - Check title length
   - Check business rules
   
3. Database Constraints (Infrastructure Layer)
   - Database triggers
   - Unique constraints
```

### 4. Error Handling

```csharp
// Domain: Business exceptions
public class InvalidTaskException : Exception { }

// Shared: Custom exceptions
public class NotFoundException : Exception { }
public class ValidationException : Exception { }

// Application: Catches and translates
try
{
    entity.MarkAsCompleted();
}
catch (InvalidTaskException ex)
{
    throw new ValidationException(ex.Message);
}

// API: Handles HTTP response
catch (ValidationException ex)
{
    return BadRequest(ex.Message);
}
```

### 5. Testing Strategy

```
Unit Tests (TaskServiceTests)
 └─ Test business logic
 └─ Mock repository
 └─ Fast execution
 └─ Test what service does

Integration Tests (TasksControllerIntegrationTests)
 └─ Test complete flow
 └─ Real HTTP requests
 └─ In-memory database
 └─ Test how layers work together
```

### 6. When to Use Each Pattern

| Pattern | When | Why |
|---------|------|-----|
| **Repository** | Abstracting data access | Testability, swappable implementations |
| **Service** | Coordinating business logic | Reusability, separation of concerns |
| **DTO** | Transferring between layers | Security, flexibility |
| **Validator** | Validating input | Centralized rules, reusability |
| **Dependency Injection** | Wiring components | Loose coupling, testability |

---

## Anti-Patterns to Avoid

### ❌ Don't: Couple to Implementation

```csharp
// BAD: Depends on concrete class
public class TaskService
{
    private TaskRepository _repo = new TaskRepository();
}
```

### ✅ Do: Use Dependency Injection

```csharp
// GOOD: Depends on interface
public class TaskService
{
    private readonly ITaskRepository _repo;
    
    public TaskService(ITaskRepository repo)
    {
        _repo = repo;
    }
}
```

### ❌ Don't: Expose Entities

```csharp
// BAD: Returns entity directly
[HttpGet]
public IActionResult GetTask(int id)
{
    return Ok(_service.GetTask(id));  // Entity!
}
```

### ✅ Do: Use DTOs

```csharp
// GOOD: Returns DTO
[HttpGet]
public IActionResult GetTask(int id)
{
    var dto = _service.GetTask(id);  // DTO!
    return Ok(dto);
}
```

### ❌ Don't: Put Business Logic in Controller

```csharp
// BAD: Business logic in controller
[HttpPost]
public IActionResult CreateTask(CreateTaskDto dto)
{
    if (dto.Title.Length < 3)
        return BadRequest();
    if (dto.Title.Length > 200)
        return BadRequest();
        
    // ... more logic
}
```

### ✅ Do: Use Service Layer

```csharp
// GOOD: Business logic in service
[HttpPost]
public async Task<IActionResult> CreateTask(CreateTaskDto dto)
{
    var result = await _service.CreateTaskAsync(dto);
    return Ok(result);
}
```

---

## Summary

Clean Architecture provides a blueprint for building **maintainable, testable, and flexible applications**.

**Key Principles:**
- Separate concerns into layers
- Dependencies flow inward
- Interfaces for abstraction
- DTOs for boundaries
- Dependency injection for coupling
- Tests at multiple levels

**Result:** An application where:
- Business logic is independent
- Database can be swapped
- Layers can be tested in isolation
- Code is easy to understand and modify

---

## Next Steps

1. **Read the Code** - Each file has detailed comments
2. **Run Tests** - See unit and integration tests in action
3. **Make Changes** - Try adding a new field to `TaskItem`
4. **Experiment** - Create a new entity (Category) using the same patterns
5. **Build** - Use this architecture for your own projects

---

**Happy coding and learning! 🚀**
