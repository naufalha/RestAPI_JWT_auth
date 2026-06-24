using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using jwt_rest_api.Data;
using jwt_rest_api.Services;
using jwt_rest_api.Services.Authentication;
using jwt_rest_api.Middlewares;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure OpenAPI
builder.Services.AddOpenApi();

// Configure PostgreSQL Database (Supabase)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GameDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Configure Dependency Injection for Authentication Strategies
builder.Services.AddScoped<IAuthStrategy, GoogleAuthStrategy>();
builder.Services.AddScoped<IAuthStrategy, TestAuthStrategy>();
builder.Services.AddScoped<IAuthStrategy, EmailAuthStrategy>();
builder.Services.AddScoped<IAuthStrategyFactory, AuthStrategyFactory>();

// Configure Dependency Injection for Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IGameService, GameService>();

// Tambahkan baris ini untuk menghidupkan AutoMapper!
builder.Services.AddAutoMapper(typeof(Program));
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
        Console.WriteLine("Supabase PostgreSQL Database initialization checked/completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Warning: Database auto-initialization skipped or failed. Ensure your Supabase connection string is correct. Error: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => 
    {
        options.WithTitle("Game REST API");
    });
}

// Tambahkan middleware logging request custom
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
