using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using jwt_rest_api.Data;
using jwt_rest_api.Models;

namespace jwt_rest_api.Repositories;

public class GameProgressRepository : IGameProgressRepository
{
    private readonly GameDbContext _dbContext;

    public GameProgressRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GameProgress?> GetByUserIdAsync(string userId)
    {
        return await _dbContext.GameProgresses
            .FirstOrDefaultAsync(gp => gp.UserId == userId);
    }

    public void Add(GameProgress progress)
    {
        _dbContext.GameProgresses.Add(progress);
    }
}
