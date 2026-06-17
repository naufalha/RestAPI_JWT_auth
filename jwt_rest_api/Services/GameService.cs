using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using jwt_rest_api.Common;
using jwt_rest_api.Data;
using jwt_rest_api.Models;
using jwt_rest_api.Models.Dto;
using jwt_rest_api.Services.Authentication;

namespace jwt_rest_api.Services;

public class GameService : IGameService
{
    private readonly GameDbContext _dbContext;

    public GameService(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<User>> GetOrCreateUserAsync(StrategyUserPayload payload)
    {
        try
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.GoogleSubjectId == payload.SubjectId);

            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = payload.Email,
                    Name = payload.Name,
                    GoogleSubjectId = payload.SubjectId,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Users.Add(user);
                
                // Initialize default game progress for new user
                var defaultProgress = new GameProgress
                {
                    UserId = user.Id,
                    Level = 1,
                    Score = 0,
                    Coins = 100, // starting coins
                    InventoryJson = "[]",
                    StateDataJson = "{}",
                    LastUpdated = DateTime.UtcNow
                };
                
                _dbContext.GameProgresses.Add(defaultProgress);

                await _dbContext.SaveChangesAsync();
            }

            return Result<User>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<User>.Failure(ResultType.Failure, $"Failed to get or create user: {ex.Message}");
        }
    }

    public async Task<Result<GameProgress>> GetProgressAsync(string userId)
    {
        try
        {
            var progress = await _dbContext.GameProgresses
                .FirstOrDefaultAsync(gp => gp.UserId == userId);

            if (progress == null)
            {
                return Result<GameProgress>.NotFound($"Game progress for user ID '{userId}' was not found.");
            }

            return Result<GameProgress>.Success(progress);
        }
        catch (Exception ex)
        {
            return Result<GameProgress>.Failure(ResultType.Failure, $"Failed to retrieve game progress: {ex.Message}");
        }
    }

    public async Task<Result> SaveProgressAsync(string userId, GameProgressDto progressDto)
    {
        try
        {
            var progress = await _dbContext.GameProgresses
                .FirstOrDefaultAsync(gp => gp.UserId == userId);

            if (progress == null)
            {
                return Result.NotFound($"Game progress for user ID '{userId}' was not found.");
            }

            // Serialize Inventory
            var inventoryJson = JsonSerializer.Serialize(progressDto.Inventory);
            
            // Serialize StateData
            var stateDataJson = JsonSerializer.Serialize(progressDto.StateData);

            progress.Level = progressDto.Level;
            progress.Score = progressDto.Score;
            progress.Coins = progressDto.Coins;
            progress.InventoryJson = inventoryJson;
            progress.StateDataJson = stateDataJson;
            progress.LastUpdated = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ResultType.Failure, $"Failed to save game progress: {ex.Message}");
        }
    }
}
