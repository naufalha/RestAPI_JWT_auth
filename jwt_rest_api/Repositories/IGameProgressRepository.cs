using System.Threading.Tasks;
using jwt_rest_api.Models;

namespace jwt_rest_api.Repositories;

public interface IGameProgressRepository
{
    // Mengambil progress berdasarkan ID User
    Task<GameProgress?> GetByUserIdAsync(string userId);
    
    // Menambahkan progress baru
    void Add(GameProgress progress);
}
