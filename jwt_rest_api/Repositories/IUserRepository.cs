using System.Collections.Generic;
using System.Threading.Tasks;
using jwt_rest_api.Models;

namespace jwt_rest_api.Repositories;

// Ini adalah "Kontrak" yang harus dipenuhi oleh Si Tukang Gudang User
public interface IUserRepository
{
    // Mengambil user berdasarkan ID Google
    Task<User?> GetByGoogleSubjectIdAsync(string subjectId);
    
    // Mengambil total jumlah user (untuk dashboard)
    Task<int> GetTotalUsersAsync();
    
    // Menambahkan user baru
    void Add(User user);
    
    // Mengambil semua user beserta data progress-nya (untuk dashboard)
    Task<List<(User User, GameProgress? Progress)>> GetUsersWithProgressAsync();
}
