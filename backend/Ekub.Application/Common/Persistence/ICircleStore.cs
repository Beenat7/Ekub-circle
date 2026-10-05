using Ekub.Domain.Entities;

namespace Ekub.Application.Common.Persistence;

public interface ICircleStore
{
    Task<List<Circle>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Circle>> GetAvailableCirclesAsync(CancellationToken cancellationToken);
    Task<List<Circle>> GetUserCirclesAsync(int memberId, CancellationToken cancellationToken);
    Task<Circle?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Circle?> GetByIdWithMembersAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Circle circle, CancellationToken cancellationToken);
    Task<bool> IsMemberInCircleAsync(int circleId, int memberId, CancellationToken cancellationToken);
    Task AddMemberAsync(CircleMember circleMember, CancellationToken cancellationToken);
}
