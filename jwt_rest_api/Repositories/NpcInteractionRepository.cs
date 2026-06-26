using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using jwt_rest_api.Data;
using jwt_rest_api.Models;

namespace jwt_rest_api.Repositories;

public class NpcInteractionRepository : INpcInteractionRepository
{
    private readonly GameDbContext _dbContext;

    public NpcInteractionRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(NpcInteraction interaction)
    {
        await _dbContext.NpcInteractions.AddAsync(interaction);
    }

    public async Task<List<NpcInteraction>> GetByUserIdAsync(string userId)
    {
        return await _dbContext.NpcInteractions
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.Timestamp)
            .ToListAsync();
    }

    public async Task<List<NpcInteraction>> GetAllAsync()
    {
        return await _dbContext.NpcInteractions.ToListAsync();
    }
}
