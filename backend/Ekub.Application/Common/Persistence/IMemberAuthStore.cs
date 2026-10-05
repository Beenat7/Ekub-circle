using Ekub.Domain.Entities;

namespace Ekub.Application.Common.Persistence;

public interface IMemberAuthStore
{
    Task<Member?> FindByUsernameAsync(
        string username,
        CancellationToken cancellationToken);

    Task<Member?> FindByPhoneNumberAsync(
        string phoneNumber,
        CancellationToken cancellationToken);

    Task AddAsync(
        Member member,
        CancellationToken cancellationToken);
}
