using Ekub.Application.Common.Persistence;
using Ekub.Domain.Entities;
using Ekub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ekub.Infrastructure.Persistence;

public sealed class CircleStore(EkubDbContext dbContext) : ICircleStore
{
    public Task<List<Circle>> GetAllAsync(CancellationToken cancellationToken)
    {
        return dbContext.Circles
            .AsNoTracking()
            .Include(c => c.Organizer)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Circle>> GetByMemberIdAsync(
        int memberId,
        CancellationToken cancellationToken)
    {
        return dbContext.Circles
            .AsNoTracking()
            .Include(c => c.Organizer)
            .Where(c => c.CircleMembers.Any(cm => cm.MemberId == memberId))
            .ToListAsync(cancellationToken);
    }

    public Task<Circle?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return dbContext.Circles
            .AsNoTracking()
            .Include(c => c.Organizer)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(Circle circle, CancellationToken cancellationToken)
    {
        dbContext.Circles.Add(circle);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
