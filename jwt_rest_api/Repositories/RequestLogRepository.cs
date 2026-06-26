using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using jwt_rest_api.Data;

namespace jwt_rest_api.Repositories;

public class RequestLogRepository : IRequestLogRepository
{
    private readonly GameDbContext _dbContext;

    public RequestLogRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> GetTotalRequestsAsync()
    {
        return await _dbContext.RequestLogs.CountAsync();
    }

    public async Task<List<int>> GetRecentTrafficAsync(int minutes)
    {
        var since = DateTime.UtcNow.AddMinutes(-minutes);
        var logs = await _dbContext.RequestLogs
            .Where(l => l.Timestamp >= since)
            .Select(l => l.Timestamp)
            .ToListAsync();

        var traffic = new List<int>(new int[minutes + 1]);
        var now = DateTime.UtcNow;
        
        foreach (var log in logs)
        {
            var diff = (int)(now - log).TotalMinutes;
            if (diff >= 0 && diff <= minutes)
            {
                traffic[minutes - diff]++;
            }
        }
        return traffic;
    }
}
