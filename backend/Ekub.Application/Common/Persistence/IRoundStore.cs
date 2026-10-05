using Ekub.Domain.Entities;

namespace Ekub.Application.Common.Persistence;

public interface IRoundStore
{
    Task<List<Round>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Round>> GetByCircleIdAsync(int circleId, CancellationToken cancellationToken);
    Task<Round?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Round round, CancellationToken cancellationToken);
}
