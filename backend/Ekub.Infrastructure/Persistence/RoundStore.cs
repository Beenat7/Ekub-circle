using Ekub.Application.Common.Persistence;
using Ekub.Domain.Entities;
using Ekub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ekub.Infrastructure.Persistence;

public sealed class RoundStore(EkubDbContext dbContext) : IRoundStore
{
    public Task<List<Round>> GetAllAsync(CancellationToken cancellationToken)
    {
        return dbContext.Rounds
            .AsNoTracking()
            .Include(r => r.Circle)
            .Include(r => r.ReceiverMember)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Round>> GetByCircleIdAsync(int circleId, CancellationToken cancellationToken)
    {
        return dbContext.Rounds
            .AsNoTracking()
            .Where(r => r.CircleId == circleId)
            .ToListAsync(cancellationToken);
    }

    public Task<Round?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return dbContext.Rounds
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task AddAsync(Round round, CancellationToken cancellationToken)
    {
        dbContext.Rounds.Add(round);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
