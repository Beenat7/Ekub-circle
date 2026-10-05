using Ekub.Application.Common.Persistence;
using Ekub.Domain.Entities;
using Ekub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ekub.Infrastructure.Persistence;

public sealed class PaymentStore(EkubDbContext dbContext) : IPaymentStore
{
    public Task<List<Payment>> GetAllAsync(CancellationToken cancellationToken)
    {
        return dbContext.Payments
            .AsNoTracking()
            .Include(p => p.Member)
            .Include(p => p.Circle)
            .Include(p => p.Round)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Payment>> GetByRoundIdAsync(int roundId, CancellationToken cancellationToken)
    {
        return dbContext.Payments
            .AsNoTracking()
            .Include(p => p.Member)
            .Where(p => p.RoundId == roundId)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Payment>> GetByMemberIdAsync(int memberId, CancellationToken cancellationToken)
    {
        return dbContext.Payments
            .AsNoTracking()
            .Where(p => p.MemberId == memberId)
            .ToListAsync(cancellationToken);
    }

    public Task<Payment?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return dbContext.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken)
    {
        dbContext.Payments.Add(payment);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
