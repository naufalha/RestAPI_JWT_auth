using System;
using System.Threading.Tasks;

namespace jwt_rest_api.Repositories;

// Mandor Gudang: Memastikan semua perubahan disimpan sekaligus
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IGameProgressRepository GameProgresses { get; }
    IRequestLogRepository RequestLogs { get; }
    INpcInteractionRepository NpcInteractions { get; }
    IGameSessionRepository GameSessions { get; }

    Task<int> SaveChangesAsync();
}
