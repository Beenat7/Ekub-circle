using Ekub.Application.Common.Persistence;
using Ekub.Domain.Entities;
using Ekub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ekub.Infrastructure.Persistence;

public sealed class CircleMemberStore(EkubDbContext dbContext) : ICircleMemberStore
{
    public Task<List<CircleMember>> GetByCircleIdAsync(int circleId, CancellationToken cancellationToken)
    {
        return dbContext.CircleMembers
            .AsNoTracking()
            .Include(cm => cm.Member)
            .Where(cm => cm.CircleId == circleId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CircleMember circleMember, CancellationToken cancellationToken)
    {
        dbContext.CircleMembers.Add(circleMember);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
