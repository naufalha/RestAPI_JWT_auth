using System.Collections.Generic;
using System.Threading.Tasks;
using jwt_rest_api.Models;

namespace jwt_rest_api.Repositories;

public interface IGameSessionRepository
{
    Task<IEnumerable<GameSession>> GetAllWithDetailsAsync();
}
