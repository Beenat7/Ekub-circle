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
            .Include(c => c.CircleMembers)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Circle>> GetAvailableCirclesAsync(CancellationToken cancellationToken)
    {
        return dbContext.Circles
            .AsNoTracking()
            .Include(c => c.Organizer)
            .Include(c => c.CircleMembers)
            .Where(c => c.Status == CircleStatus.NotStarted && c.CircleMembers.Count < c.MaxMembers)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Circle>> GetUserCirclesAsync(int memberId, CancellationToken cancellationToken)
    {
        return dbContext.Circles
            .AsNoTracking()
            .Include(c => c.Organizer)
            .Include(c => c.CircleMembers)
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

    public Task<Circle?> GetByIdWithMembersAsync(int id, CancellationToken cancellationToken)
    {
        return dbContext.Circles
            .Include(c => c.Organizer)
            .Include(c => c.CircleMembers)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(Circle circle, CancellationToken cancellationToken)
    {
        dbContext.Circles.Add(circle);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> IsMemberInCircleAsync(int circleId, int memberId, CancellationToken cancellationToken)
    {
        return dbContext.CircleMembers
            .AnyAsync(cm => cm.CircleId == circleId && cm.MemberId == memberId, cancellationToken);
    }

    public async Task AddMemberAsync(CircleMember circleMember, CancellationToken cancellationToken)
    {
        dbContext.CircleMembers.Add(circleMember);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
