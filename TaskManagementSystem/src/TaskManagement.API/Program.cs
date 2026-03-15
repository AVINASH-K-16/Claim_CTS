using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;
using TaskManagement.Application.Validators;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Data.DbContext;
using TaskManagement.Infrastructure.Repositories;

var builder = WebApplicationBuilder.CreateBuilder(args);

// ===================================================================
// DEPENDENCY INJECTION CONFIGURATION
// ===================================================================
// This is where we tell ASP.NET Core which implementations to use
// When a controller asks for ITaskService, give it TaskService
// When a class asks for ITaskRepository, give it TaskRepository
// ===================================================================

// Add controllers
builder.Services.AddControllers();

// Configure Entity Framework Core with In-Memory Database
// For production, use: UseSqlServer(connectionString)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TaskManagementDb")
);

// Register the Repository (Infrastructure Layer)
// When ITaskRepository is needed, provide TaskRepository
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Register the Validator (Application Layer)
builder.Services.AddScoped<TaskValidator>();

// Register the Service (Application Layer)
// When ITaskService is needed, provide TaskService
// TaskService will receive ITaskRepository and TaskValidator via constructor injection
builder.Services.AddScoped<ITaskService, TaskService>();

// Configure Swagger/OpenAPI for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===================================================================
// BUILD THE APPLICATION
// ===================================================================
var app = builder.Build();

// Auto-create database and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

// ===================================================================
// DEPENDENCY INJECTION EXPLAINED
// ===================================================================
//
// When an HTTP request arrives:
// 1. Controller needs ITaskService
// 2. ASP.NET Core looks up ITaskService in the DI container
// 3. Finds that TaskService implements ITaskService
// 4. TaskService needs ITaskRepository, finds TaskRepository
// 5. Creates TaskService(taskRepository) and injects it
// 6. Controller executes the method
//
// This decouples all layers!
// ===================================================================
