using Ekub.Application.Common.Persistence;
using Ekub.Domain.Entities;
using Ekub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ekub.Infrastructure.Persistence;

public sealed class MemberAuthStore(EkubDbContext dbContext) : IMemberAuthStore
{
    public Task<Member?> FindByUsernameAsync(
        string username,
        CancellationToken cancellationToken)
    {
        return dbContext.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(
                member => member.Username == username,
                cancellationToken);
    }

    public Task<Member?> FindByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken)
    {
        return dbContext.Members
            .FirstOrDefaultAsync(
                member => member.PhoneNumber == phoneNumber,
                cancellationToken);
    }

    public async Task AddAsync(
        Member member,
        CancellationToken cancellationToken)
    {
        dbContext.Members.Add(member);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
