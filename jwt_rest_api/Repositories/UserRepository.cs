using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using jwt_rest_api.Data;
using jwt_rest_api.Models;

namespace jwt_rest_api.Repositories;

public class UserRepository : IUserRepository
{
    // Hanya repository yang boleh memegang DbContext
    private readonly GameDbContext _dbContext;

    public UserRepository(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByGoogleSubjectIdAsync(string subjectId)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.GoogleSubjectId == subjectId);
    }

    public async Task<int> GetTotalUsersAsync()
    {
        return await _dbContext.Users.CountAsync();
    }

    public void Add(User user)
    {
        _dbContext.Users.Add(user);
        // Perhatikan: Kita TIDAK memanggil _dbContext.SaveChangesAsync() di sini.
        // Nanti kita akan panggil di UnitOfWork!
    }

    public async Task<List<(User User, GameProgress? Progress)>> GetUsersWithProgressAsync()
    {
        // Query kompleks pindah ke sini
        var usersWithProgress = await _dbContext.Users
            .GroupJoin(
                _dbContext.GameProgresses,
                u => u.Id,
                gp => gp.UserId,
                (u, gp) => new { User = u, Progresses = gp }
            )
            .SelectMany(
                x => x.Progresses.DefaultIfEmpty(),
                (x, gp) => new { x.User, Progress = gp }
            )
            .ToListAsync();

        var resultList = new List<(User, GameProgress?)>();
        foreach (var item in usersWithProgress)
        {
            resultList.Add((item.User, item.Progress));
        }
        
        return resultList;
    }
}
