# Task Management System - Clean Architecture Learning Project

> **A beginner-friendly ASP.NET project demonstrating Enterprise-Level Clean Architecture**

## 📚 What This Project Is

This is a **learning project** that teaches you how to build scalable, maintainable applications using **Clean Architecture** principles in .NET. It demonstrates:

✅ Separation of concerns (each layer has one responsibility)  
✅ Dependency injection (loose coupling between layers)  
✅ Repository pattern (abstraction over database)  
✅ Service layer (business logic isolation)  
✅ DTOs (data transfer objects for security and flexibility)  
✅ Unit & integration tests  
✅ REST API design  
✅ MVC web application  

---

## 🏗️ Architecture Overview

### The 6-Layer Clean Architecture

```
┌─────────────────────────────────────────────────────────┐
│         PRESENTATION LAYERS (User Interface)             │
├──────────────────────┬──────────────────────────────────┤
│  TaskManagement.API  │  TaskManagement.Web              │
│  (REST Endpoints)    │  (Razor Pages/MVC)               │
└──────────────────────┴──────────────────────────────────┘
                        ▼
┌─────────────────────────────────────────────────────────┐
│   APPLICATION LAYER (Business Logic & Orchestration)    │
│         TaskManagement.Application                      │
│  - Services, DTOs, Commands, Queries, Validators       │
└─────────────────────────────────────────────────────────┘
                        ▼
┌─────────────────────────────────────────────────────────┐
│   INFRASTRUCTURE LAYER (Data Access & External Services)│
│      TaskManagement.Infrastructure                      │
│  - Database Context, Repositories, Entity Configs       │
└─────────────────────────────────────────────────────────┘
                        ▼
┌─────────────────────────────────────────────────────────┐
│      DOMAIN LAYER (Core Business Rules)                 │
│      TaskManagement.Domain                              │
│  - Entities, Enums, Interfaces, Value Objects           │
└─────────────────────────────────────────────────────────┘

┌──────────────────────────────────────┐
│  SHARED LAYER (Utilities & Helpers)  │
│  TaskManagement.Shared               │
│  - Exceptions, Constants, Extensions │
└──────────────────────────────────────┘
```

---

## 📁 Project Structure

```
TaskManagementSystem/
├── src/
│   ├── TaskManagement.Domain/
│   │   ├── Entities/        → TaskItem.cs
│   │   ├── Enums/          → TaskStatus.cs
│   │   ├── Interfaces/     → ITaskRepository.cs
│   │   └── ValueObjects/   → (Future domain values)
│   │
│   ├── TaskManagement.Shared/
│   │   ├── Exceptions/     → Custom exceptions
│   │   ├── Constants/      → AppConstants.cs
│   │   ├── Extensions/     → String helpers
│   │   └── Helpers/        → ValidationHelper.cs
│   │
│   ├── TaskManagement.Application/
│   │   ├── Interfaces/     → ITaskService.cs
│   │   ├── Services/       → TaskService.cs
│   │   ├── DTOs/          → TaskDto.cs, CreateTaskDto.cs
│   │   ├── Validators/    → TaskValidator.cs
│   │   ├── Commands/      → (Entity commands)
│   │   └── Queries/       → (Entity queries)
│   │
│   ├── TaskManagement.Infrastructure/
│   │   ├── Data/
│   │   │   └── DbContext/ → ApplicationDbContext.cs
│   │   ├── Repositories/  → TaskRepository.cs
│   │   └── Services/      → (External services)
│   │
│   ├── TaskManagement.API/
│   │   ├── Controllers/   → TasksController.cs
│   │   └── Program.cs     → DI Configuration
│   │
│   └── TaskManagement.Web/
│       ├── Controllers/   → TaskController.cs
│       ├── Views/         → Razor templates
│       ├── ViewModels/    → UI models
│       └── Program.cs     → DI Configuration
│
├── tests/
│   ├── TaskManagement.UnitTests/
│   │   └── TaskServiceTests.cs
│   │
│   └── TaskManagement.IntegrationTests/
│       └── TasksControllerIntegrationTests.cs
│
└── docs/
    └── architecture.md
```

---

## 🔑 Key Concepts Explained

### 1. **Domain Layer** (`TaskManagement.Domain`)

**Purpose:** Contains core business logic and data models

**Key Files:**
- `TaskItem.cs` - Core business entity
- `TaskStatus.cs` - Enum of task states
- `ITaskRepository.cs` - Contract for data access

**Why Separate?**
- Independent of databases and frameworks
- Can be used in console apps, APIs, web apps
- Easy to test business rules in isolation

```csharp
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
    
    // Business logic methods
    public void MarkAsCompleted() => IsCompleted = true;
}
```

### 2. **Shared Layer** (`TaskManagement.Shared`)

**Purpose:** Utilities and constants used across all layers

**Files:**
- `CustomExceptions.cs` - `NotFoundException`, `ValidationException`
- `AppConstants.cs` - Validation rules, error messages
- `StringExtensions.cs` - Helper methods
- `ValidationHelper.cs` - Validation logic

**Why Separate?**
- No dependencies on other layers
- Reusable everywhere
- Easy to modify business rules globally

### 3. **Application Layer** (`TaskManagement.Application`)

**Purpose:** Business logic orchestration

**Key Files:**
- `ITaskService.cs` - Interface defining operations
- `TaskService.cs` - Implementation (coordinates domain & infrastructure)
- `TaskDto.cs` - Data transfer objects
- `TaskValidator.cs` - Input validation

**Data Flow Example:**
```
API Request → TaskService.CreateTask()
    ↓
Validation (TaskValidator)
    ↓
Create Entity (TaskItem)
    ↓
Persist to DB (Repository)
    ↓
Convert to DTO (TaskDto)
    ↓
Return to API
```

**Why DTOs?**
- Hide internal implementation details
- Security (don't expose all fields)
- Convert between layers without coupling
- Flexibility to change entity without breaking API

### 4. **Infrastructure Layer** (`TaskManagement.Infrastructure`)

**Purpose:** Database access and external services

**Key Files:**
- `ApplicationDbContext.cs` - Entity Framework configuration
- `TaskRepository.cs` - Implements `ITaskRepository`

**Key Benefit:**
Database code is completely separate. To switch databases:
```csharp
// Old: SQL Server
options.UseSqlServer("connection-string")

// New: PostgreSQL
options.UseNpgsql("connection-string")
```
No changes needed in other layers!

### 5. **API Layer** (`TaskManagement.API`)

**Purpose:** REST endpoints for external clients

**REST Endpoints:**
```
GET    /api/tasks              → Get all tasks
GET    /api/tasks/{id}         → Get task by ID
POST   /api/tasks              → Create task
PUT    /api/tasks/{id}         → Update task
DELETE /api/tasks/{id}         → Delete task
POST   /api/tasks/{id}/complete → Mark completed
GET    /api/tasks/completed    → Get completed tasks
```

**Key File:**
- `TasksController.cs` - Handles HTTP requests
- `Program.cs` - Dependency injection setup

### 6. **Web Layer** (`TaskManagement.Web`)

**Purpose:** MVC/Razor pages for browser users

**Key Files:**
- `TaskController.cs` - MVC controller
- `Views/Task/` - Razor HTML templates
- `ViewModels/` - Data for views

**Note:** Both API and Web layers use the same `ITaskService`!

---

## 🔗 Data Flow: Request → Response

### Example: Creating a Task

```
CLIENT REQUEST: POST /api/tasks
        ↓
API LAYER (TasksController)
   - Receives HTTP request
   - Validates input
        ↓
APPLICATION LAYER (TaskService)
   - Validates business rules
   - Creates domain entity
   - Calls repository to save
        ↓
INFRASTRUCTURE LAYER (TaskRepository)
   - Database operations
   - Entity Framework Core
        ↓
DOMAIN LAYER (TaskItem Entity)
   - Business logic methods
        ↓
DATABASE
   - Persists data (SQL Server/InMemory)
        ↓
RETURN (same path back up)
   - Convert Entity → DTO
   - Return HTTP 201 Created
        ↓
CLIENT RESPONSE: TaskDto with new ID
```

---

## 💉 Dependency Injection Explained

ASP.NET Core automatically wires up dependencies for you:

```csharp
// In Program.cs
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

// In Controller
public TasksController(ITaskService taskService)
{
    _taskService = taskService;  // Automatically injected!
}

// Inside TaskService
public TaskService(ITaskRepository taskRepository)
{
    _taskRepository = taskRepository;  // Automatically injected!
}
```

**Flow:**
1. Controller asks for `ITaskService`
2. Framework looks at `Program.cs`
3. Finds `TaskService` implements `ITaskService`
4. `TaskService` needs `ITaskRepository`, finds `TaskRepository`
5. Creates `TaskRepository(dbContext)` → `TaskService(repository)` → `TaskController(service)`
6. Done! Everything is wired up!

**Benefit:** Complete loose coupling. Easy to mock for testing!

---

## 🧪 Testing

### Unit Tests (`TaskManagement.UnitTests`)

**What:** Test business logic in isolation  
**How:** Mock the repository, test only service logic  
**Speed:** Fast ⚡ (no database)

```csharp
[Fact]
public async Task CreateTask_WithValidData_ShouldCreateTask()
{
    // Arrange: Setup mock data
    var createDto = new CreateTaskDto { Title = "Learn Clean Arch" };
    _mockRepository.Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
        .ReturnsAsync(new TaskItem { Id = 1 });

    // Act: Execute service method
    var result = await _service.CreateTaskAsync(createDto);

    // Assert: Verify result
    Assert.NotNull(result);
    Assert.Equal(1, result.Id);
}
```

### Integration Tests (`TaskManagement.IntegrationTests`)

**What:** Test complete flow (API → Service → Database)  
**How:** Create real HTTP requests, use in-memory database  
**Speed:** Slower 🐢 (but more realistic)

```csharp
[Fact]
public async Task CreateTask_ShouldPersistToDatabase()
{
    var createDto = new CreateTaskDto { Title = "Learn Clean Arch" };
    
    var response = await _client.PostAsJsonAsync("/api/tasks", createDto);
    
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
}
```

---

## 🚀 How to Run

### Prerequisites
- .NET 8 SDK installed
- Visual Studio Code or Visual Studio 2022

### Option 1: API Only
```bash
cd src/TaskManagement.API
dotnet run
# Opens on http://localhost:5001
# Swagger at http://localhost:5001/swagger
```

### Option 2: Web Application
```bash
cd src/TaskManagement.Web
dotnet run
# Opens on http://localhost:5001
```

### Option 3: Run Both (different terminals)
```bash
# Terminal 1
cd src/TaskManagement.API && dotnet run

# Terminal 2
cd src/TaskManagement.Web && dotnet run
```

### Run Tests
```bash
# Unit tests
dotnet test tests/TaskManagement.UnitTests/

# Integration tests
dotnet test tests/TaskManagement.IntegrationTests/
```

---

## 📊 Testing the API

### Using REST Client in VS Code

Install the "REST Client" extension, then create a `requests.http` file:

```http
### Get all tasks
GET http://localhost:5001/api/tasks

### Create a new task
POST http://localhost:5001/api/tasks
Content-Type: application/json

{
  "title": "Learn Entity Framework",
  "description": "Understand ORM and migrations"
}

### Get task by ID
GET http://localhost:5001/api/tasks/1

### Update a task
PUT http://localhost:5001/api/tasks/1
Content-Type: application/json

{
  "title": "Master Entity Framework",
  "description": "Learn advanced queries and relationships"
}

### Mark as completed
POST http://localhost:5001/api/tasks/1/complete

### Delete a task
DELETE http://localhost:5001/api/tasks/1
```

---

## 🎯 Design Patterns Used

| Pattern | Location | What It Does |
|---------|----------|--------------|
| **Repository** | Infrastructure | Abstracts database operations |
| **Service** | Application | Orchestrates business logic |
| **Dependency Injection** | All layers | Loose coupling, easy testing |
| **Data Transfer Object** | Application/API | Transfers data between layers |
| **Value Object** | Domain | Represents domain values |
| **Factory** | Infrastructure | Creates complex objects |

---

## 📚 Learning Path

### Week 1: Foundations
1. Understand the 6 layers
2. Read through each layer's code comments
3. Run the API and make requests

### Week 2: Services & Repositories
1. Modify `TaskValidator` to add new rules
2. Add a new method to `ITaskRepository` (Soft Delete)
3. Implement it in `TaskRepository`

### Week 3: Features
1. Add `Priority` field to `TaskItem`
2. Create `GetByPriority` method
3. Add validation rules
4. Update controller endpoint

### Week 4: Testing
1. Write unit tests for new validation
2. Write integration tests for new endpoint
3. Verify all existing tests still pass

---

## 🔄 Common Tasks

### Add a New Entity (e.g., Category)

1. **Domain Layer**: Create `Category.cs` entity
2. **Infrastructure**: Create `CategoryRepository.cs`, add `DbSet<Category>` to context
3. **Application**: Create `ICategoryService.cs` and `CategoryService.cs`
4. **API**: Create `CategoriesController.cs`
5. **Tests**: Create unit and integration tests
6. **Web**: Create views if needed

### Change Database (SQL Server → PostgreSQL)

Only change `Program.cs`:
```csharp
// Old (SQL Server)
options.UseSqlServer("Server=...;Database=...")

// New (PostgreSQL)
options.UseNpgsql("Host=...;Database=...")
```

All other layers remain unchanged! 🎉

### Add Authentication

1. Add Identity to `ApplicationDbContext`
2. Create `IAuthService.cs` in Application
3. Create `AuthController.cs` in API
4. Update `Program.cs` dependency injection

---

## ⚠️ Common Mistakes to Avoid

❌ **Don't:** Call database directly from controller  
✅ **Do:** Inject `ITaskService` and use it

❌ **Don't:** Return entities from API  
✅ **Do:** Convert to DTOs in service

❌ **Don't:** Put business logic in controller  
✅ **Do:** Put it in service or domain entity

❌ **Don't:** Create tight coupling between layers  
✅ **Do:** Use dependency injection and interfaces

---

## 🎓 Key Takeaways

1. **Separation of Concerns**: Each layer has one job
2. **Dependency Injection**: Loose coupling makes testing easy
3. **Repository Pattern**: Database changes don't affect business logic
4. **DTOs**: Protect entities, improve API design
5. **Testing**: Unit tests are fast, integration tests are comprehensive
6. **Configuration**: All wiring is in `Program.cs`

---

## 📖 Resources for Learning

- [Microsoft Clean Architecture Docs](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/)
- [Repository Pattern Explanation](https://martinfowler.com/eaaCatalog/repository.html)
- [Dependency Injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

---

## 📝 License

This is a learning project. Use it freely for educational purposes!

---

## 🤝 Questions?

Each file contains detailed comments explaining:
- What the code does
- Why it's structured that way
- How it connects to other layers

Read the comments first—they're your best teacher! 📚

---

**Happy Learning!** 🚀
