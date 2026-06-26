using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using jwt_rest_api.Common;
using jwt_rest_api.Data;
using jwt_rest_api.Models;
using jwt_rest_api.Models.Dto;
using jwt_rest_api.Services.Authentication;
using AutoMapper;
using jwt_rest_api.Repositories;


namespace jwt_rest_api.Services;

public class GameService : IGameService
{
    private readonly IUnitOfWork _unitOfWork; // Panggil Mandor
    private readonly IMapper _mapper; // Tambahkan ini

    public GameService(IUnitOfWork unitOfWork, IMapper mapper) // Tambahkan IMapper di sini
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper; // Tambahkan ini
    }

    public async Task<Result<User>> GetOrCreateUserAsync(StrategyUserPayload payload)
    {
        try
        {
           var user = await _unitOfWork.Users.GetByGoogleSubjectIdAsync(payload.SubjectId);
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

                _unitOfWork.Users.Add(user);
                
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
                
                _unitOfWork.GameProgresses.Add(defaultProgress);

                await _unitOfWork.SaveChangesAsync();
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
            var progress = await _unitOfWork.GameProgresses.GetByUserIdAsync(userId);

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
            // 1. Basic Validation
            if (progressDto.Level <= 0)
            {
                return Result.ValidationError("Level must be greater than 0.");
            }
            if (progressDto.Coins < 0)
            {
                return Result.ValidationError("Coins cannot be negative.");
            }

            // 2. StateData Validation (Day between 1-10, Phase between 1-4)
            if (progressDto.StateData.ValueKind == JsonValueKind.Object)
            {
                if (progressDto.StateData.TryGetProperty("Day", out var dayProp))
                {
                    if (dayProp.ValueKind == JsonValueKind.Number && dayProp.TryGetInt32(out var day))
                    {
                        if (day < 1 || day > 10)
                        {
                            return Result.ValidationError("Day must be between 1 and 10.");
                        }
                    }
                    else
                    {
                        return Result.ValidationError("Day must be a valid number.");
                    }
                }
                if (progressDto.StateData.TryGetProperty("Phase", out var phaseProp))
                {
                    if (phaseProp.ValueKind == JsonValueKind.Number && phaseProp.TryGetInt32(out var phase))
                    {
                        if (phase < 1 || phase > 4)
                        {
                            return Result.ValidationError("Phase must be between 1 and 4.");
                        }
                    }
                    else
                    {
                        return Result.ValidationError("Phase must be a valid number.");
                    }
                }
            }

            var progress = await _unitOfWork.GameProgresses.GetByUserIdAsync(userId);

            if (progress == null)
            {
                return Result.NotFound($"Game progress for user ID '{userId}' was not found.");
            }

            // 3. Optimistic Concurrency Control (OCC) Check
            if (progressDto.LastUpdated.HasValue)
            {
                var dbTime = progress.LastUpdated.ToUniversalTime();
                var clientTime = progressDto.LastUpdated.Value.ToUniversalTime();
                if (dbTime - clientTime > TimeSpan.FromMilliseconds(100))
                {
                    return Result.Conflict("Sesi penyimpanan konflik. Data di server telah diperbarui oleh sesi lain. Silakan muat ulang.");
                }
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

            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ResultType.Failure, $"Failed to save game progress: {ex.Message}");
        }
    }

    public async Task<Result<AdminDashboardStatsDto>> GetAdminDashboardDataAsync()
    {
        try
        {
            var totalUsers = await _unitOfWork.Users.GetTotalUsersAsync();
            var totalRequests = await _unitOfWork.RequestLogs.GetTotalRequestsAsync();

            var usersWithProgress = await _unitOfWork.Users.GetUsersWithProgressAsync();

            var playerStats = new List<PlayerStatDto>();

            foreach (var item in usersWithProgress)
            {
                var stat = new PlayerStatDto
                {
                    Id = item.User.Id,
                    Name = item.User.Name,
                    Email = item.User.Email,
                    LastUpdated = item.User.CreatedAt
                };

                if (item.Progress != null)
                {
                    stat.Level = item.Progress.Level;
                    stat.Score = item.Progress.Score;
                    stat.Coins = item.Progress.Coins;
                    stat.LastUpdated = item.Progress.LastUpdated;

                    // Parse StateDataJson
                    try
                    {
                        using var doc = JsonDocument.Parse(item.Progress.StateDataJson);
                        if (doc.RootElement.TryGetProperty("Day", out var dayProp) && dayProp.ValueKind == JsonValueKind.Number)
                        {
                            stat.DayReached = dayProp.GetInt32();
                        }
                        else
                        {
                            stat.DayReached = 1;
                        }

                        if (doc.RootElement.TryGetProperty("Phase", out var phaseProp) && phaseProp.ValueKind == JsonValueKind.Number)
                        {
                            stat.PhaseReached = phaseProp.GetInt32();
                        }
                        else
                        {
                            stat.PhaseReached = 1;
                        }
                    }
                    catch
                    {
                        stat.DayReached = 1;
                        stat.PhaseReached = 1;
                    }

                    // For Interactions, we can mock it based on Level * Score
                    stat.TotalInteractions = stat.Level * 2 + (stat.Score / 10);
                    
                    // Simple logic for PsychoProfile based on Score
                    if (stat.Score > 50) stat.PsychoProfile = "Empathetic";
                    else if (stat.Score < 10 && stat.Level > 3) stat.PsychoProfile = "Pragmatic";
                    else stat.PsychoProfile = "Neutral";
                }

                playerStats.Add(stat);
            }

            var dashboardData = new AdminDashboardStatsDto
            {
                TotalUsers = totalUsers,
                TotalRequests = totalRequests,
                TrafficData = await _unitOfWork.RequestLogs.GetRecentTrafficAsync(5),
                Players = playerStats
            };

            return Result<AdminDashboardStatsDto>.Success(dashboardData);
        }
        catch (Exception ex)
        {
            return Result<AdminDashboardStatsDto>.Failure(ResultType.Failure, $"Failed to retrieve admin dashboard data: {ex.Message}");
        }
    }

}
