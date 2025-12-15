using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LibrarySystem.Data;
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("DB_CONNECTION")));

var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? throw new InvalidOperationException("Secret key not found in environment variables.");

builder.Services.AddAuthentication(
    options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => 
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    }
);

var app = builder.Build();

// Initialize and seed database with admin user
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    dbContext.Database.Migrate();

    if(!dbContext.Users.Any(i => i.Roles == "admin") )
    {
        Console.WriteLine("No admin user found. Creating default admin user...");
        var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL");
        if(string.IsNullOrEmpty(adminEmail))
        {
            throw new InvalidOperationException("Admin email not set in environment variables.");
        }
        var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
        if(string.IsNullOrEmpty(adminPassword))
        {
            throw new InvalidOperationException("Admin password not set in environment variables.");
        }

        var adminUser = new LibrarySystem.Models.User
        {
            FirstName = "Admin",
            LastName = "Admin",
            Email = adminEmail,
            Password = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            Roles = "admin"
        };

        dbContext.Users.Add(adminUser);
        dbContext.SaveChanges();
        Console.WriteLine("Admin user created with email: " + adminEmail);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
