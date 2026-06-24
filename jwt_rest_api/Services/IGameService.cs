using System.Threading.Tasks;
using jwt_rest_api.Common;
using jwt_rest_api.Models;
using jwt_rest_api.Models.Dto;
using jwt_rest_api.Services.Authentication;

namespace jwt_rest_api.Services;

public interface IGameService
{
    Task<Result<User>> GetOrCreateUserAsync(StrategyUserPayload payload);
    Task<Result<GameProgress>> GetProgressAsync(string userId);
    Task<Result> SaveProgressAsync(string userId, GameProgressDto progressDto);
    Task<Result<AdminDashboardStatsDto>> GetAdminDashboardDataAsync();
}
