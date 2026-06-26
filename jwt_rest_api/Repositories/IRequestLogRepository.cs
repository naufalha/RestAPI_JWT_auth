using System.Collections.Generic;
using System.Threading.Tasks;

namespace jwt_rest_api.Repositories;

public interface IRequestLogRepository
{
    Task<int> GetTotalRequestsAsync();
    Task<List<int>> GetRecentTrafficAsync(int minutes);
}
