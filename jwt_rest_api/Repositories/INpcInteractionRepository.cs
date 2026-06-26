using System.Collections.Generic;
using System.Threading.Tasks;
using jwt_rest_api.Models;

namespace jwt_rest_api.Repositories;

public interface INpcInteractionRepository
{
    Task AddAsync(NpcInteraction interaction);
    Task<List<NpcInteraction>> GetByUserIdAsync(string userId);
    Task<List<NpcInteraction>> GetAllAsync();
}
