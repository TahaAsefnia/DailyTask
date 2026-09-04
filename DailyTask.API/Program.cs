using DailyTask.API.Data;
using DailyTask.API.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Services

builder.Services.AddOpenApi();
builder.Services.AddDbContext<DailyTaskDbContext>(options => options.UseSqlite("Data Source=DailyTaskDB.db"));
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(p => p
        .WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
        .AllowAnyHeader()
        .AllowAnyMethod()));

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
app.UseCors();

app.MapTaskEndpoints();
app.Run();
