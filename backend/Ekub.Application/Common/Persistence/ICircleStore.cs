using Ekub.Domain.Entities;

namespace Ekub.Application.Common.Persistence;

public interface ICircleStore
{
    Task<List<Circle>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Circle>> GetByMemberIdAsync(int memberId, CancellationToken cancellationToken);
    Task<Circle?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Circle circle, CancellationToken cancellationToken);
}
