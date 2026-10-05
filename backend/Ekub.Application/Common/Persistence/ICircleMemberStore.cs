using Ekub.Domain.Entities;

namespace Ekub.Application.Common.Persistence;

public interface ICircleMemberStore
{
    Task<List<CircleMember>> GetByCircleIdAsync(int circleId, CancellationToken cancellationToken);
    Task AddAsync(CircleMember circleMember, CancellationToken cancellationToken);
}
