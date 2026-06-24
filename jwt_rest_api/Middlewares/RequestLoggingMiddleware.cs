using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using jwt_rest_api.Data;
using jwt_rest_api.Models;

namespace jwt_rest_api.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Panggil request selanjutnya
        await _next(context);

        var path = context.Request.Path.Value ?? string.Empty;

        // Hanya catat log untuk endpoint API (/api/...) 
        // Abaikan endpoint dashboard (/api/admin/dashboard) dan file statis (html, js, dll)
        if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) || 
            path.StartsWith("/api/admin/dashboard", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // Catat request setelah selesai untuk mendapatkan StatusCode
        var log = new RequestLog
        {
            Method = context.Request.Method,
            Path = context.Request.Path,
            StatusCode = context.Response.StatusCode,
            Timestamp = DateTime.UtcNow
        };

        // Gunakan service scope untuk mendapatkan dbContext
        var dbContext = context.RequestServices.GetRequiredService<GameDbContext>();
        dbContext.RequestLogs.Add(log);
        await dbContext.SaveChangesAsync();
    }
}
