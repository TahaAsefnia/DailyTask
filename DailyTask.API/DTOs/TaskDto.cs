using System.ComponentModel.DataAnnotations;
using DailyTask.API.Entities;

namespace DailyTask.API.DTOs;

public record TaskDto(
    string Id,
    string Title,
    string Description,
    Priority Priority,
    bool IsCompleted);