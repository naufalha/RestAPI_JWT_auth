using System;
using System.Threading.Tasks;
using jwt_rest_api.Data;

namespace jwt_rest_api.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly GameDbContext _dbContext;

    private IUserRepository? _users;
    private IGameProgressRepository? _gameProgresses;
    private IRequestLogRepository? _requestLogs;
    private INpcInteractionRepository? _npcInteractions;
    private IGameSessionRepository? _gameSessions;

    public IUserRepository Users => _users ??= new UserRepository(_dbContext);
    public IGameProgressRepository GameProgresses => _gameProgresses ??= new GameProgressRepository(_dbContext);
    public IRequestLogRepository RequestLogs => _requestLogs ??= new RequestLogRepository(_dbContext);
    public INpcInteractionRepository NpcInteractions => _npcInteractions ??= new NpcInteractionRepository(_dbContext);
    public IGameSessionRepository GameSessions => _gameSessions ??= new GameSessionRepository(_dbContext);

    public UnitOfWork(GameDbContext dbContext)
    {
        _dbContext = dbContext;
        
        // Inisialisasi semua repository di sini, dengan memberikan kunci gudang (_dbContext) yang sama
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
