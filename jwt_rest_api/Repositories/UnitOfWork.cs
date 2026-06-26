using System;
using System.Threading.Tasks;
using jwt_rest_api.Data;

namespace jwt_rest_api.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly GameDbContext _dbContext;

    public IUserRepository Users { get; private set; }
    public IGameProgressRepository GameProgresses { get; private set; }
    public IRequestLogRepository RequestLogs { get; private set; }

    public UnitOfWork(GameDbContext dbContext)
    {
        _dbContext = dbContext;
        
        // Inisialisasi semua repository di sini, dengan memberikan kunci gudang (_dbContext) yang sama
        Users = new UserRepository(_dbContext);
        GameProgresses = new GameProgressRepository(_dbContext);
        RequestLogs = new RequestLogRepository(_dbContext);
    }

    public async Task<int> SaveChangesAsync()
    {
        // Menyimpan semua perubahan (dari semua repository) dalam satu waktu!
        return await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
