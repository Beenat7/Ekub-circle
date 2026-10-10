using Ekub.Application.Common;
using Ekub.Domain.Entities;

namespace Ekub.Application.Common.Persistence;

public interface IRoundLifecycleStore
{
    Task<Result<Circle>> AddCircleWithOrganizerAsync(
        Circle circle,
        CancellationToken cancellationToken);

    Task<Result<Circle>> LockCircleAsync(
        int circleId,
        int memberId,
        CancellationToken cancellationToken);

    Task<Result<Round>> OpenNextRoundAsync(
        int circleId,
        int memberId,
        CancellationToken cancellationToken);

    Task<Result<CircleMember>> AddCircleMemberAsync(
        CircleMember circleMember,
        CancellationToken cancellationToken);

    Task<Result<Payment>> AddPaymentAsync(
        Payment payment,
        CancellationToken cancellationToken);

    Task<Result<Payment>> ReviewPaymentAsync(
        int paymentId,
        int reviewerId,
        bool approve,
        CancellationToken cancellationToken);

    Task<Result<Payout>> AddPayoutAndAdvanceRoundAsync(
        Payout payout,
        CancellationToken cancellationToken);
}
