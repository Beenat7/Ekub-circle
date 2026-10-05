using Ekub.Domain.Entities;

namespace Ekub.Application.Common.Persistence;

public interface IPaymentStore
{
    Task<List<Payment>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Payment>> GetByRoundIdAsync(int roundId, CancellationToken cancellationToken);
    Task<List<Payment>> GetByMemberIdAsync(int memberId, CancellationToken cancellationToken);
    Task<Payment?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Payment payment, CancellationToken cancellationToken);
}
