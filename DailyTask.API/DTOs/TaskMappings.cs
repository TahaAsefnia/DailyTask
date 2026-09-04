// DTOs/TaskMappings.cs
using DailyTask.API.DTOs;
using Task = DailyTask.API.Entities.Task;

namespace DailyTask.API.DTOs;

public static class TaskMappings
{
    
    public static TaskDto ToDto(this Task task) =>
        new(task.Id, task.Title, task.Description, task.Priority, task.IsCompleted);
    
    public static Task ToEntity(this CreateTaskDto dto) =>
        new()
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            IsCompleted = false   
        };

    public static void ToEntity(this UpdateTaskDto dto, Task task)
    {
        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Priority = dto.Priority;
        task.IsCompleted = dto.IsCompleted;
    }
}