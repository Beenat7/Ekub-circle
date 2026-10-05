using Ekub.Application.Common.Persistence;
using Ekub.Domain.Entities;
using Ekub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ekub.Infrastructure.Persistence;

public sealed class PayoutStore(EkubDbContext dbContext) : IPayoutStore
{
    public Task<List<Payout>> GetAllAsync(CancellationToken cancellationToken)
    {
        return dbContext.Payouts
            .AsNoTracking()
            .Include(p => p.Member)
            .Include(p => p.Circle)
            .Include(p => p.Round)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Payout>> GetByRoundIdAsync(int roundId, CancellationToken cancellationToken)
    {
        return dbContext.Payouts
            .AsNoTracking()
            .Where(p => p.RoundId == roundId)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Payout>> GetByMemberIdAsync(int memberId, CancellationToken cancellationToken)
    {
        return dbContext.Payouts
            .AsNoTracking()
            .Where(p => p.MemberId == memberId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Payout payout, CancellationToken cancellationToken)
    {
        dbContext.Payouts.Add(payout);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
