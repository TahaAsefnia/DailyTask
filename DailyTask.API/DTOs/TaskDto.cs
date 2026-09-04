using DailyTask.API.Entities;

namespace DailyTask.API.DTOs;

public record TaskDto(
    int Id,
    string Title,
    string Description,
    Priority Priority,
    bool IsCompleted);