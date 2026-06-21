using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using jwt_rest_api.Data;
using jwt_rest_api.Services;
using jwt_rest_api.Services.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure OpenAPI
builder.Services.AddOpenApi();

// Configure MySQL Database using Pomelo EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GameDbContext>(options =>
{
    ServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 30)); // fallback
    try
    {
        if (!string.IsNullOrEmpty(connectionString))
        {
            serverVersion = ServerVersion.AutoDetect(connectionString);
        }
    }
    catch
    {
        // Fallback to standard version if MySQL server is offline during build
    }
    
    options.UseMySql(connectionString, serverVersion);
});

// Configure Dependency Injection for Authentication Strategies
builder.Services.AddScoped<IAuthStrategy, GoogleAuthStrategy>();
builder.Services.AddScoped<IAuthStrategy, TestAuthStrategy>();
builder.Services.AddScoped<IAuthStrategy, EmailAuthStrategy>();
builder.Services.AddScoped<IAuthStrategyFactory, AuthStrategyFactory>();

// Configure Dependency Injection for Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IGameService, GameService>();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] 
    ?? throw new InvalidOperationException("JWT Signing Key is not configured in appsettings.json.");
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "GameBackendServer",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "UnityGameClient",
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero // Immediate expiration validation
    };
});

var app = builder.Build();

// Automatically initialize database schema if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<GameDbContext>();
        context.Database.EnsureCreated();
        Console.WriteLine("MySQL Database initialization checked/completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Warning: Database auto-initialization skipped or failed. Ensure your connection string is correct and MySQL is running. Error: {ex.Message}");
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
