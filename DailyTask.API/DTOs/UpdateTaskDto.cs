using System.ComponentModel.DataAnnotations;
using DailyTask.API.Entities;

namespace DailyTask.API.DTOs;

public record UpdateTaskDto(
    [Required]
    [MaxLength(25)]
    string Title,
    [Required]
    [MaxLength(500)]
    string Description,
    [EnumDataType(typeof(Priority))]
    Priority Priority,
    bool IsCompleted
    );