using Microsoft.EntityFrameworkCore;
using Task = DailyTask.API.Entities.Task;

namespace DailyTask.API.Data;

public class DailyTaskDbContext(DbContextOptions<DailyTaskDbContext> options)
    : DbContext(options)
{
    public DbSet<Task> Tasks => Set<Task>();
}