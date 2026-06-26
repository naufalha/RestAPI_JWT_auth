using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using jwt_rest_api.Data;
using jwt_rest_api.Models;

namespace jwt_rest_api.Repositories;

public class GameSessionRepository : IGameSessionRepository
{
    private readonly GameDbContext _dbContext;

    public GameSessionRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<GameSession>> GetAllWithDetailsAsync()
    {
        return await _dbContext.GameSessions
            .Include(s => s.Events)
                .ThenInclude(e => e.Encounters)
                    .ThenInclude(en => en.PlayerResponses)
            .ToListAsync();
    }
}
