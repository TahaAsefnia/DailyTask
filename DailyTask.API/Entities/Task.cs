namespace DailyTask.API.Entities;

public class Task
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Priority Priority{ get; set; } =  Priority.Normal;
    public bool IsCompleted { get; set; }
}
