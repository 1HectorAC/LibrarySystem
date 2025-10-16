using Microsoft.EntityFrameworkCore;
using DotNetEnv;

using LibrarySystem.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

Env.Load();
builder.Services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNECTION")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
