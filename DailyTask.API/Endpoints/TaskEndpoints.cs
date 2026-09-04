namespace DailyTask.API.Endpoints;

public static class TaskEndpoints
{
    static WebApplication MapTaskEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/tasks");
        

        return app;
    } 
}