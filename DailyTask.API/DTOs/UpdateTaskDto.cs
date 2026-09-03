using System.ComponentModel.DataAnnotations;
using DailyTask.API.Entities;

namespace DailyTask.API.DTOs;

public record UpdateTaskDto(
    [MaxLength(25)]
    string Title,
    [MaxLength(500)]
    string Description,
    Priority Priority,
    bool IsCompleted
    );