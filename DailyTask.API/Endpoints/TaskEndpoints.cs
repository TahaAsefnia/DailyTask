using DailyTask.API.Data;
using DailyTask.API.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DailyTask.API.Endpoints;

public static class TaskEndpoints
{
    public static WebApplication MapTaskEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/tasks").WithTags("Tasks").WithParameterValidation();

        group.MapGet("/", GetAllTasks);
        group.MapGet("/{id:int}", GetTaskById);
        group.MapPost("/", CreateTask);
        group.MapPut("/{id:int}", UpdateTask);
        group.MapPatch("/{id:int}/toggle", ToggleTask);
        group.MapDelete("/{id:int}", DeleteTask);

        return app;
    }

    private static async Task<Ok<List<TaskDto>>> GetAllTasks(DailyTaskDbContext db)
    {
        var tasks = await db.Tasks
            .OrderBy(t => t.IsCompleted)          
            .ThenByDescending(t => t.Priority)
            .ToListAsync();

        return TypedResults.Ok(tasks.Select(task => task.ToDto()).ToList());
    }

    private static async Task<Results<Ok<TaskDto>, NotFound>> GetTaskById(int id, DailyTaskDbContext db)
    {
        var task = await db.Tasks.FindAsync(id);

        return task is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(task.ToDto());
    }

    private static async Task<Created<TaskDto>> CreateTask(CreateTaskDto dto, DailyTaskDbContext db)
    {
        var task = dto.ToEntity();

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/tasks/{task.Id}", task.ToDto());
    }

    private static async Task<Results<Ok<TaskDto>, NotFound>> UpdateTask(int id, UpdateTaskDto dto, DailyTaskDbContext db)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null)
            return TypedResults.NotFound();

        dto.ToEntity(task);

        await db.SaveChangesAsync();

        return TypedResults.Ok(task.ToDto());
    }

    private static async Task<Results<Ok<TaskDto>, NotFound>> ToggleTask(int id, DailyTaskDbContext db)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null)
            return TypedResults.NotFound();

        task.IsCompleted = !task.IsCompleted;   

        await db.SaveChangesAsync();

        return TypedResults.Ok(task.ToDto());
    }

    private static async Task<Results<NoContent, NotFound>> DeleteTask(int id, DailyTaskDbContext db)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task is null)
            return TypedResults.NotFound();

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
}