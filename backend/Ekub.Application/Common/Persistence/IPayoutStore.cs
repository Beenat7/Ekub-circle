using Ekub.Domain.Entities;

namespace Ekub.Application.Common.Persistence;

public interface IPayoutStore
{
    Task<List<Payout>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Payout>> GetByRoundIdAsync(int roundId, CancellationToken cancellationToken);
    Task<List<Payout>> GetByMemberIdAsync(int memberId, CancellationToken cancellationToken);
    Task AddAsync(Payout payout, CancellationToken cancellationToken);
}
