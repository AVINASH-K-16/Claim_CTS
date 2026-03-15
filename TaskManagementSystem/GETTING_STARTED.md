## Getting Started with Clean Architecture

This project demonstrates enterprise-level ASP.NET Clean Architecture.

### Quick Start

#### 1. **Run the API**
```bash
cd src/TaskManagement.API
dotnet run
```
- API available at: `http://localhost:5001`
- Swagger UI at: `http://localhost:5001/swagger`

#### 2. **Run the Web Application**
```bash
cd src/TaskManagement.Web
dotnet run
```
- Web app available at: `http://localhost:5001`
- Task management UI ready to use

#### 3. **Run Tests**
```bash
# Unit Tests
dotnet test tests/TaskManagement.UnitTests/

# Integration Tests
dotnet test tests/TaskManagement.IntegrationTests/
```

---

### Project Structure

```
src/
├── TaskManagement.Domain/       → Business entities and rules
├── TaskManagement.Shared/       → Cross-cutting utilities
├── TaskManagement.Application/  → Business logic orchestration
├── TaskManagement.Infrastructure/ → Database access
├── TaskManagement.API/          → REST API endpoints
└── TaskManagement.Web/          → MVC web application

tests/
├── TaskManagement.UnitTests/        → Service logic tests
└── TaskManagement.IntegrationTests/ → End-to-end API tests

docs/
└── architecture.md              → Detailed architecture explanation
```

---

### API Endpoints

**Get All Tasks**
```http
GET http://localhost:5001/api/tasks
```

**Get Task by ID**
```http
GET http://localhost:5001/api/tasks/1
```

**Create Task**
```http
POST http://localhost:5001/api/tasks
Content-Type: application/json

{
  "title": "Learn Clean Architecture",
  "description": "Understand the layers"
}
```

**Update Task**
```http
PUT http://localhost:5001/api/tasks/1
Content-Type: application/json

{
  "title": "Master Clean Architecture",
  "description": "Apply in real projects"
}
```

**Mark as Completed**
```http
POST http://localhost:5001/api/tasks/1/complete
```

**Delete Task**
```http
DELETE http://localhost:5001/api/tasks/1
```

**Get Completed Tasks**
```http
GET http://localhost:5001/api/tasks/completed
```

---

### Key Files to Read

1. **Start Here:**
   - `README.md` - Overview and learning path
   - `docs/architecture.md` - Detailed architecture explanation

2. **Domain Layer:**
   - `src/TaskManagement.Domain/Entities/TaskItem.cs`
   - `src/TaskManagement.Domain/Interfaces/ITaskRepository.cs`

3. **Application Layer:**
   - `src/TaskManagement.Application/Services/TaskService.cs`
   - `src/TaskManagement.Application/DTOs/TaskDto.cs`

4. **Infrastructure Layer:**
   - `src/TaskManagement.Infrastructure/Repositories/TaskRepository.cs`
   - `src/TaskManagement.Infrastructure/Data/DbContext/ApplicationDbContext.cs`

5. **API Layer:**
   - `src/TaskManagement.API/Controllers/TasksController.cs`
   - `src/TaskManagement.API/Program.cs`

6. **Tests:**
   - `tests/TaskManagement.UnitTests/TaskServiceTests.cs`
   - `tests/TaskManagement.IntegrationTests/TasksControllerIntegrationTests.cs`

---

### Learning Objectives

✅ Understand 6-layer Clean Architecture  
✅ Learn Repository Pattern  
✅ Master Dependency Injection  
✅ Use DTOs effectively  
✅ Write unit and integration tests  
✅ Build maintainable applications  

---

### Next Steps

1. Run the API and explore endpoints with Swagger
2. Run tests to see how testing works
3. Read the code comments carefully
4. Modify the code (add new fields, methods)
5. Write your own tests
6. Apply this architecture to your projects

---

**Each file contains detailed comments explaining the "why" behind the code. Read them!** 📚
