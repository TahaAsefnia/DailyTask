using DailyTask.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Services

builder.Services.AddOpenApi();
builder.Services.AddDbContext<DailyTaskDbContext>(options => options.UseSqlite("Data Source=DailyTaskDB.db"));


var app = builder.Build();



// Configure the HTTP request pipeline.
/*
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
*/




app.Run();
