using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;
using Ekub.Domain.Entities;
using Ekub.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Ekub.Infrastructure.Persistence;

public sealed class RoundLifecycleStore(EkubDbContext dbContext) : IRoundLifecycleStore
{
    public async Task<Result<Circle>> AddCircleWithOrganizerAsync(
        Circle circle,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Members.AnyAsync(m => m.Id == circle.OrganizerId, cancellationToken))
        {
            return Result<Circle>.Failure("Organizer not found.", StatusCodes.Status404NotFound);
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        circle.CircleMembers.Add(new CircleMember
        {
            Circle = circle,
            MemberId = circle.OrganizerId,
            OrderNumber = 1,
            JoinedAt = DateTime.UtcNow
        });

        circle.Status = CircleStatus.Forming;
        dbContext.Circles.Add(circle);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Result<Circle>.Success(circle, StatusCodes.Status201Created);
    }

    public async Task<Result<Circle>> LockCircleAsync(
        int circleId,
        int memberId,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var circle = await dbContext.Circles
            .Include(c => c.CircleMembers)
            .FirstOrDefaultAsync(c => c.Id == circleId, cancellationToken);
        if (circle is null)
        {
            return Result<Circle>.Failure("Circle not found.", StatusCodes.Status404NotFound);
        }

        if (circle.OrganizerId != memberId)
        {
            return Result<Circle>.Failure(
                "Only the circle creator can start this circle.",
                StatusCodes.Status403Forbidden);
        }

        if (circle.Status != CircleStatus.Forming && circle.Status != CircleStatus.NotStarted)
        {
            return Result<Circle>.Failure(
                "This circle has already started or completed.",
                StatusCodes.Status400BadRequest);
        }

        var members = circle.CircleMembers
            .OrderBy(cm => cm.OrderNumber)
            .ToList();

        if (members.Count < 2)
        {
            return Result<Circle>.Failure(
                "At least two members must join before the circle can be started.",
                StatusCodes.Status400BadRequest);
        }

        if (!members.Any(cm => cm.MemberId == circle.OrganizerId))
        {
            return Result<Circle>.Failure(
                "The circle organizer must be included as a circle member.",
                StatusCodes.Status400BadRequest);
        }

        var now = DateTime.UtcNow;
        circle.Status = CircleStatus.Active;
        circle.StartedAt = now;

        var existingRounds = await dbContext.Rounds
            .Where(r => r.CircleId == circleId)
            .ToListAsync(cancellationToken);

        if (!existingRounds.Any())
        {
            for (int i = 0; i < members.Count; i++)
            {
                var roundNumber = i + 1;
                var round = new Round
                {
                    CircleId = circleId,
                    RoundNumber = roundNumber,
                    ReceiverMemberId = members[i].MemberId,
                    StartedAt = i == 0 ? now : now.AddDays(circle.ContributionIntervalDays * i),
                    DueDate = now.AddDays(circle.ContributionIntervalDays * (i + 1)),
                    Status = i == 0 ? RoundStatus.Open : RoundStatus.Pending
                };
                dbContext.Rounds.Add(round);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Result<Circle>.Success(circle);
    }

    public async Task<Result<Round>> OpenNextRoundAsync(
        int circleId,
        int memberId,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var circle = await dbContext.Circles
            .Include(c => c.CircleMembers)
            .FirstOrDefaultAsync(c => c.Id == circleId, cancellationToken);
        if (circle is null)
        {
            return Result<Round>.Failure("Circle not found.", StatusCodes.Status404NotFound);
        }

        if (circle.OrganizerId != memberId)
        {
            return Result<Round>.Failure(
                "Only the circle creator can open a round.",
                StatusCodes.Status403Forbidden);
        }

        if (circle.Status != CircleStatus.Active)
        {
            return Result<Round>.Failure(
                "Only an active circle can open a round.",
                StatusCodes.Status400BadRequest);
        }

        var openRound = await dbContext.Rounds
            .FirstOrDefaultAsync(r => r.CircleId == circleId && r.Status == RoundStatus.Open, cancellationToken);
        if (openRound is not null)
        {
            return Result<Round>.Success(openRound);
        }

        var nextPendingRound = await dbContext.Rounds
            .Where(r => r.CircleId == circleId && r.Status == RoundStatus.Pending)
            .OrderBy(r => r.RoundNumber)
            .FirstOrDefaultAsync(cancellationToken);

        if (nextPendingRound is null)
        {
            return Result<Round>.Failure(
                "No pending rounds remaining for this circle.",
                StatusCodes.Status400BadRequest);
        }

        var now = DateTime.UtcNow;
        nextPendingRound.Status = RoundStatus.Open;
        nextPendingRound.StartedAt = now;
        nextPendingRound.DueDate = now.AddDays(circle.ContributionIntervalDays);

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Result<Round>.Success(nextPendingRound);
    }

    public async Task<Result<CircleMember>> AddCircleMemberAsync(
        CircleMember circleMember,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var circle = await dbContext.Circles
            .FirstOrDefaultAsync(c => c.Id == circleMember.CircleId, cancellationToken);
        if (circle is null)
        {
            return Result<CircleMember>.Failure("Circle not found.", StatusCodes.Status404NotFound);
        }

        if (circle.Status != CircleStatus.Forming && circle.Status != CircleStatus.NotStarted)
        {
            return Result<CircleMember>.Failure(
                "This circle has already started and is no longer accepting members.",
                StatusCodes.Status400BadRequest);
        }

        if (circleMember.OrderNumber < 1 || circleMember.OrderNumber > circle.MaxMembers)
        {
            return Result<CircleMember>.Failure(
                $"Order number must be between 1 and {circle.MaxMembers}.",
                StatusCodes.Status400BadRequest);
        }

        if (!await dbContext.Members.AnyAsync(m => m.Id == circleMember.MemberId, cancellationToken))
        {
            return Result<CircleMember>.Failure("Member not found.", StatusCodes.Status404NotFound);
        }

        var currentMembers = await dbContext.CircleMembers
            .Where(cm => cm.CircleId == circleMember.CircleId)
            .ToListAsync(cancellationToken);

        if (currentMembers.Any(cm => cm.MemberId == circleMember.MemberId))
        {
            return Result<CircleMember>.Failure(
                "This member has already joined the circle.",
                StatusCodes.Status409Conflict);
        }

        if (currentMembers.Any(cm => cm.OrderNumber == circleMember.OrderNumber))
        {
            return Result<CircleMember>.Failure(
                "That payout position is already taken.",
                StatusCodes.Status409Conflict);
        }

        if (currentMembers.Count >= circle.MaxMembers)
        {
            return Result<CircleMember>.Failure("This circle is full.", StatusCodes.Status409Conflict);
        }

        dbContext.CircleMembers.Add(circleMember);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Result<CircleMember>.Success(circleMember, StatusCodes.Status201Created);
    }

    public async Task<Result<Payment>> AddPaymentAsync(
        Payment payment,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var round = await dbContext.Rounds
            .Include(r => r.Circle)
            .FirstOrDefaultAsync(r => r.Id == payment.RoundId, cancellationToken);
        if (round is null)
        {
            return Result<Payment>.Failure("Round not found.", StatusCodes.Status404NotFound);
        }

        if (round.CircleId != payment.CircleId)
        {
            return Result<Payment>.Failure(
                "The payment circle does not match the selected round.",
                StatusCodes.Status400BadRequest);
        }

        if (round.Status != RoundStatus.Open)
        {
            return Result<Payment>.Failure("This round is not open for payments.", StatusCodes.Status400BadRequest);
        }

        if (round.Circle.Status != CircleStatus.Active)
        {
            return Result<Payment>.Failure(
                "Payments are only accepted for an active circle.",
                StatusCodes.Status400BadRequest);
        }

        if (payment.Amount != round.Circle.ContributionAmount)
        {
            return Result<Payment>.Failure(
                $"Each member must contribute {round.Circle.ContributionAmount}.",
                StatusCodes.Status400BadRequest);
        }

        var isCircleMember = await dbContext.CircleMembers.AnyAsync(
            cm => cm.CircleId == round.CircleId && cm.MemberId == payment.MemberId,
            cancellationToken);
        if (!isCircleMember)
        {
            return Result<Payment>.Failure(
                "Only members of this circle can pay into its round.",
                StatusCodes.Status403Forbidden);
        }

        if (payment.RecordedBy != payment.MemberId && payment.RecordedBy != round.Circle.OrganizerId)
        {
            return Result<Payment>.Failure(
                "A member can only record their own contribution unless performed by the organizer.",
                StatusCodes.Status403Forbidden);
        }

        if (string.IsNullOrWhiteSpace(payment.BankName) ||
            string.IsNullOrWhiteSpace(payment.TransactionId))
        {
            return Result<Payment>.Failure(
                "Bank or provider name and transaction ID are required.",
                StatusCodes.Status400BadRequest);
        }

        var existingPayment = await dbContext.Payments
            .FirstOrDefaultAsync(
                p => p.RoundId == round.Id && p.MemberId == payment.MemberId,
                cancellationToken);
        if (existingPayment is not null && existingPayment.Status != PaymentStatus.Rejected)
        {
            return Result<Payment>.Failure(
                "A payment for this member and round is already awaiting review or has been approved.",
                StatusCodes.Status409Conflict);
        }

        var member = await dbContext.Members
            .FirstOrDefaultAsync(m => m.Id == payment.RecordedBy, cancellationToken);
        if (member is null)
        {
            return Result<Payment>.Failure("Recorded-by member not found.", StatusCodes.Status404NotFound);
        }

        payment.Circle = round.Circle;
        payment.Member = member;

        if (existingPayment is null)
        {
            payment.Status = PaymentStatus.Pending;
            dbContext.Payments.Add(payment);
        }
        else
        {
            existingPayment.Amount = payment.Amount;
            existingPayment.PaymentMethod = payment.PaymentMethod;
            existingPayment.BankName = payment.BankName;
            existingPayment.TransactionId = payment.TransactionId;
            existingPayment.PaidAt = DateTime.UtcNow;
            existingPayment.RecordedBy = payment.RecordedBy;
            existingPayment.RecordedAt = DateTime.UtcNow;
            existingPayment.Status = PaymentStatus.Pending;
            existingPayment.ReviewedByMemberId = null;
            existingPayment.ReviewedAt = null;
            payment = existingPayment;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Result<Payment>.Success(payment, StatusCodes.Status201Created);
    }

    public async Task<Result<Payment>> ReviewPaymentAsync(
        int paymentId,
        int reviewerId,
        bool approve,
        CancellationToken cancellationToken)
    {
        var payment = await dbContext.Payments
            .Include(p => p.Circle)
            .Include(p => p.Round)
            .Include(p => p.Member)
            .FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
        if (payment is null)
        {
            return Result<Payment>.Failure("Payment not found.", StatusCodes.Status404NotFound);
        }

        if (payment.Circle.OrganizerId != reviewerId)
        {
            return Result<Payment>.Failure(
                "Only the circle creator can review this payment.",
                StatusCodes.Status403Forbidden);
        }

        if (payment.Status != PaymentStatus.Pending)
        {
            return Result<Payment>.Failure(
                "This payment has already been reviewed.",
                StatusCodes.Status400BadRequest);
        }

        if (payment.Circle.Status != CircleStatus.Active ||
            payment.Round.Status != RoundStatus.Open)
        {
            return Result<Payment>.Failure(
                "Payments can only be reviewed while their circle round is open.",
                StatusCodes.Status400BadRequest);
        }

        payment.Status = approve ? PaymentStatus.Approved : PaymentStatus.Rejected;
        payment.ReviewedByMemberId = reviewerId;
        payment.ReviewedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result<Payment>.Success(payment);
    }

    public async Task<Result<Payout>> AddPayoutAndAdvanceRoundAsync(
        Payout payout,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var round = await dbContext.Rounds
            .Include(r => r.Circle)
            .FirstOrDefaultAsync(r => r.Id == payout.RoundId, cancellationToken);
        if (round is null)
        {
            return Result<Payout>.Failure("Round not found.", StatusCodes.Status404NotFound);
        }

        if (round.CircleId != payout.CircleId)
        {
            return Result<Payout>.Failure(
                "The payout circle does not match the selected round.",
                StatusCodes.Status400BadRequest);
        }

        // Rule D: No repeat payout for a completed round
        if (round.Status != RoundStatus.Open)
        {
            return Result<Payout>.Failure("This round is not open for payout.", StatusCodes.Status400BadRequest);
        }

        if (round.Circle.Status != CircleStatus.Active)
        {
            return Result<Payout>.Failure("Payouts can only be recorded for active circles.", StatusCodes.Status400BadRequest);
        }

        // Rule B: Fixed payout order - Recipient must match assigned round receiver
        if (payout.MemberId != round.ReceiverMemberId)
        {
            return Result<Payout>.Failure(
                "The payout recipient must be the member assigned to this round.",
                StatusCodes.Status400BadRequest);
        }

        // Rule C: No duplicate winner in the circle
        var hasAlreadyWon = await dbContext.Payouts
            .AnyAsync(p => p.CircleId == round.CircleId && p.MemberId == payout.MemberId, cancellationToken);
        if (hasAlreadyWon)
        {
            return Result<Payout>.Failure(
                "This member has already received a payout in this circle.",
                StatusCodes.Status400BadRequest);
        }

        var payoutExistsForRound = await dbContext.Payouts
            .AnyAsync(p => p.RoundId == round.Id, cancellationToken);
        if (payoutExistsForRound)
        {
            return Result<Payout>.Failure(
                "A payout has already been recorded for this round.",
                StatusCodes.Status400BadRequest);
        }

        // Rule F: Authorization - Organizer only
        if (payout.RecordedBy != round.Circle.OrganizerId)
        {
            return Result<Payout>.Failure(
                "Only the circle organizer can record a payout.",
                StatusCodes.Status403Forbidden);
        }

        var members = await dbContext.CircleMembers
            .Where(cm => cm.CircleId == round.CircleId)
            .OrderBy(cm => cm.OrderNumber)
            .ToListAsync(cancellationToken);

        // Rule A: Everyone must pay before payout (Only Approved payments count)
        var paidMemberIds = await dbContext.Payments
            .Where(p => p.RoundId == round.Id && p.Status == PaymentStatus.Approved)
            .Select(p => p.MemberId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (members.Count < 2 ||
            paidMemberIds.Count != members.Count ||
            members.Any(cm => !paidMemberIds.Contains(cm.MemberId)))
        {
            return Result<Payout>.Failure(
                "A payout can only be recorded after every circle member has paid.",
                StatusCodes.Status400BadRequest);
        }

        var expectedAmount = round.Circle.ContributionAmount * members.Count;
        if (payout.Amount != expectedAmount)
        {
            return Result<Payout>.Failure(
                $"The payout amount must equal the full pot of {expectedAmount}.",
                StatusCodes.Status400BadRequest);
        }

        if (!await dbContext.Members.AnyAsync(m => m.Id == payout.RecordedBy, cancellationToken))
        {
            return Result<Payout>.Failure("Recorded-by member not found.", StatusCodes.Status404NotFound);
        }

        var completedAt = DateTime.UtcNow;
        payout.PaidAt = completedAt;
        dbContext.Payouts.Add(payout);
        round.Status = RoundStatus.Completed;
        round.CompletedAt = completedAt;

        // Rule E: Advance to next round or mark circle Completed
        var nextRound = await dbContext.Rounds
            .FirstOrDefaultAsync(r => r.CircleId == round.CircleId && r.RoundNumber == round.RoundNumber + 1, cancellationToken);

        if (nextRound is not null)
        {
            nextRound.Status = RoundStatus.Open;
            nextRound.StartedAt = completedAt;
            nextRound.DueDate = completedAt.AddDays(round.Circle.ContributionIntervalDays);
        }
        else
        {
            round.Circle.Status = CircleStatus.Completed;
            round.Circle.CompletedAt = completedAt;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return Result<Payout>.Success(payout, StatusCodes.Status201Created);
    }
}
