using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ekub.Domain.Entities;
using Ekub.Infrastructure.Data;
using Ekub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ekub.Tests;

public class EkubLifecycleTests
{
    private EkubDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<EkubDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new EkubDbContext(options);
    }

    private async Task<(EkubDbContext dbContext, RoundLifecycleStore store, Member organizer, Member member2, Circle circle)> SeedCircleWithTwoMembersAsync()
    {
        var db = CreateInMemoryDbContext();
        var store = new RoundLifecycleStore(db);

        var organizer = new Member
        {
            Id = 1,
            FirstName = "Abreham",
            MiddleName = "Y",
            LastName = "T",
            PhoneNumber = "+251911111111",
            Username = "abreham",
            PasswordHash = "hash"
        };
        var member2 = new Member
        {
            Id = 2,
            FirstName = "Saron",
            MiddleName = "T",
            LastName = "F",
            PhoneNumber = "+251922222222",
            Username = "saron",
            PasswordHash = "hash"
        };
        db.Members.AddRange(organizer, member2);
        await db.SaveChangesAsync();

        var circle = new Circle
        {
            Id = 100,
            Name = "Family Ekub",
            ContributionAmount = 1000m,
            MaxMembers = 2,
            ContributionIntervalDays = 7,
            OrganizerId = organizer.Id,
            Organizer = organizer,
            Status = CircleStatus.Forming
        };

        var addRes = await store.AddCircleWithOrganizerAsync(circle, CancellationToken.None);
        Assert.True(addRes.IsSuccess);

        var member2Res = await store.AddCircleMemberAsync(new CircleMember
        {
            CircleId = circle.Id,
            MemberId = member2.Id,
            OrderNumber = 2
        }, CancellationToken.None);
        Assert.True(member2Res.IsSuccess);

        return (db, store, organizer, member2, circle);
    }

    [Fact]
    public async Task Scenario1_CircleWithValidMembers_CanBeStarted()
    {
        var (db, store, organizer, _, circle) = await SeedCircleWithTwoMembersAsync();

        var lockRes = await store.LockCircleAsync(circle.Id, organizer.Id, CancellationToken.None);

        Assert.True(lockRes.IsSuccess);
        Assert.Equal(CircleStatus.Active, lockRes.Value!.Status);
        Assert.NotNull(lockRes.Value.StartedAt);
    }

    [Fact]
    public async Task Scenario2_StartingCircleTwice_IsRejected()
    {
        var (db, store, organizer, _, circle) = await SeedCircleWithTwoMembersAsync();

        await store.LockCircleAsync(circle.Id, organizer.Id, CancellationToken.None);
        var secondLockRes = await store.LockCircleAsync(circle.Id, organizer.Id, CancellationToken.None);

        Assert.False(secondLockRes.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, secondLockRes.StatusCode);
    }

    [Fact]
    public async Task Scenario3_StartingUnderfilledCircle_IsRejected()
    {
        var db = CreateInMemoryDbContext();
        var store = new RoundLifecycleStore(db);

        var organizer = new Member
        {
            Id = 1,
            FirstName = "Abreham",
            MiddleName = "Y",
            LastName = "T",
            PhoneNumber = "+251911111111",
            Username = "abreham",
            PasswordHash = "hash"
        };
        db.Members.Add(organizer);
        await db.SaveChangesAsync();

        var circle = new Circle
        {
            Id = 200,
            Name = "Empty Ekub",
            ContributionAmount = 500m,
            MaxMembers = 3,
            ContributionIntervalDays = 7,
            OrganizerId = organizer.Id,
            Organizer = organizer,
            Status = CircleStatus.Forming
        };
        await store.AddCircleWithOrganizerAsync(circle, CancellationToken.None);

        // Circle only has 1 member (organizer)
        var lockRes = await store.LockCircleAsync(circle.Id, organizer.Id, CancellationToken.None);

        Assert.False(lockRes.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, lockRes.StatusCode);
    }

    [Fact]
    public async Task Scenario4_and_5_StartingGeneratesExactlyOneRoundPerMember_WithUniqueReceiver()
    {
        var (db, store, organizer, member2, circle) = await SeedCircleWithTwoMembersAsync();

        await store.LockCircleAsync(circle.Id, organizer.Id, CancellationToken.None);

        var rounds = await db.Rounds.Where(r => r.CircleId == circle.Id).OrderBy(r => r.RoundNumber).ToListAsync();
        Assert.Equal(2, rounds.Count);

        Assert.Equal(1, rounds[0].RoundNumber);
        Assert.Equal(organizer.Id, rounds[0].ReceiverMemberId);
        Assert.Equal(RoundStatus.Open, rounds[0].Status);

        Assert.Equal(2, rounds[1].RoundNumber);
        Assert.Equal(member2.Id, rounds[1].ReceiverMemberId);
        Assert.Equal(RoundStatus.Pending, rounds[1].Status);
    }

    [Fact]
    public async Task Scenario6_MembershipCannotBeChanged_AfterStarting()
    {
        var (db, store, organizer, _, circle) = await SeedCircleWithTwoMembersAsync();
        await store.LockCircleAsync(circle.Id, organizer.Id, CancellationToken.None);

        var newMember = new Member
        {
            Id = 3,
            FirstName = "Feven",
            MiddleName = "M",
            LastName = "M",
            PhoneNumber = "+251933333333",
            Username = "feven",
            PasswordHash = "hash"
        };
        db.Members.Add(newMember);
        await db.SaveChangesAsync();

        var addRes = await store.AddCircleMemberAsync(new CircleMember
        {
            CircleId = circle.Id,
            MemberId = newMember.Id,
            OrderNumber = 3
        }, CancellationToken.None);

        Assert.False(addRes.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, addRes.StatusCode);
    }

    [Fact]
    public async Task Scenario7_PayoutIsRejected_WhenOneMemberHasNotPaid()
    {
        var (db, store, organizer, member2, circle) = await SeedCircleWithTwoMembersAsync();
        await store.LockCircleAsync(circle.Id, organizer.Id, CancellationToken.None);

        var round1 = await db.Rounds.FirstAsync(r => r.CircleId == circle.Id && r.RoundNumber == 1);

        // Only organizer pays, member2 does not pay
        await store.AddPaymentAsync(new Payment
        {
            CircleId = circle.Id,
            RoundId = round1.Id,
            MemberId = organizer.Id,
            Amount = 1000m,
            BankName = "CBE",
            TransactionId = "TXN1001",
            RecordedBy = organizer.Id
        }, CancellationToken.None);

        var pm = await db.Payments.FirstAsync(p => p.RoundId == round1.Id && p.MemberId == organizer.Id);
        await store.ReviewPaymentAsync(pm.Id, organizer.Id, approve: true, CancellationToken.None);

        // Attempt payout
        var payoutRes = await store.AddPayoutAndAdvanceRoundAsync(new Payout
        {
            CircleId = circle.Id,
            RoundId = round1.Id,
            MemberId = organizer.Id,
            Amount = 2000m,
            RecordedBy = organizer.Id
        }, CancellationToken.None);

        Assert.False(payoutRes.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, payoutRes.StatusCode);
    }

    [Fact]
    public async Task Scenario8_and_14_PayoutSucceeds_WhenAllMembersPaid_AndCompletesCircleOnLastRound()
    {
        var (db, store, organizer, member2, circle) = await SeedCircleWithTwoMembersAsync();
        await store.LockCircleAsync(circle.Id, organizer.Id, CancellationToken.None);

        var round1 = await db.Rounds.FirstAsync(r => r.CircleId == circle.Id && r.RoundNumber == 1);

        // Both pay for round 1
        foreach (var mId in new[] { organizer.Id, member2.Id })
        {
            var payRes = await store.AddPaymentAsync(new Payment
            {
                CircleId = circle.Id,
                RoundId = round1.Id,
                MemberId = mId,
                Amount = 1000m,
                BankName = "CBE",
                TransactionId = $"TXN1_{mId}",
                RecordedBy = mId
            }, CancellationToken.None);
            Assert.True(payRes.IsSuccess);

            await store.ReviewPaymentAsync(payRes.Value!.Id, organizer.Id, approve: true, CancellationToken.None);
        }

        var payoutRes = await store.AddPayoutAndAdvanceRoundAsync(new Payout
        {
            CircleId = circle.Id,
            RoundId = round1.Id,
            MemberId = organizer.Id,
            Amount = 2000m,
            RecordedBy = organizer.Id
        }, CancellationToken.None);

        Assert.True(payoutRes.IsSuccess);

        // Check that Round 1 is completed and Round 2 is now Open
        var round1Refreshed = await db.Rounds.FindAsync(round1.Id);
        var round2Refreshed = await db.Rounds.FirstAsync(r => r.CircleId == circle.Id && r.RoundNumber == 2);

        Assert.Equal(RoundStatus.Completed, round1Refreshed!.Status);
        Assert.Equal(RoundStatus.Open, round2Refreshed.Status);

        // Now pay round 2 and complete circle
        foreach (var mId in new[] { organizer.Id, member2.Id })
        {
            var payRes = await store.AddPaymentAsync(new Payment
            {
                CircleId = circle.Id,
                RoundId = round2Refreshed.Id,
                MemberId = mId,
                Amount = 1000m,
                BankName = "CBE",
                TransactionId = $"TXN2_{mId}",
                RecordedBy = mId
            }, CancellationToken.None);
            await store.ReviewPaymentAsync(payRes.Value!.Id, organizer.Id, approve: true, CancellationToken.None);
        }

        var payout2Res = await store.AddPayoutAndAdvanceRoundAsync(new Payout
        {
            CircleId = circle.Id,
            RoundId = round2Refreshed.Id,
            MemberId = member2.Id,
            Amount = 2000m,
            RecordedBy = organizer.Id
        }, CancellationToken.None);

        Assert.True(payout2Res.IsSuccess);

        // Circle should now be Completed!
        var circleRefreshed = await db.Circles.FindAsync(circle.Id);
        Assert.Equal(CircleStatus.Completed, circleRefreshed!.Status);
    }

    [Fact]
    public async Task Scenario9_MemberWhoWon_MustStillPayInNextRound()
    {
        var (db, store, organizer, member2, circle) = await SeedCircleWithTwoMembersAsync();
        await store.LockCircleAsync(circle.Id, organizer.Id, CancellationToken.None);

        var round1 = await db.Rounds.FirstAsync(r => r.CircleId == circle.Id && r.RoundNumber == 1);
        foreach (var mId in new[] { organizer.Id, member2.Id })
        {
            var p = await store.AddPaymentAsync(new Payment
            {
                CircleId = circle.Id,
                RoundId = round1.Id,
                MemberId = mId,
                Amount = 1000m,
                BankName = "CBE",
                TransactionId = $"TXN1_{mId}",
                RecordedBy = mId
            }, CancellationToken.None);
            await store.ReviewPaymentAsync(p.Value!.Id, organizer.Id, approve: true, CancellationToken.None);
        }
        await store.AddPayoutAndAdvanceRoundAsync(new Payout
        {
            CircleId = circle.Id,
            RoundId = round1.Id,
            MemberId = organizer.Id,
            Amount = 2000m,
            RecordedBy = organizer.Id
        }, CancellationToken.None);

        // Organizer (previous winner) submits payment for Round 2
        var round2 = await db.Rounds.FirstAsync(r => r.CircleId == circle.Id && r.RoundNumber == 2);
        var winnerPayRes = await store.AddPaymentAsync(new Payment
        {
            CircleId = circle.Id,
            RoundId = round2.Id,
            MemberId = organizer.Id,
            Amount = 1000m,
            BankName = "CBE",
            TransactionId = "TXN2_WINNER",
            RecordedBy = organizer.Id
        }, CancellationToken.None);

        Assert.True(winnerPayRes.IsSuccess);
    }

    [Fact]
    public async Task Scenario10_and_11_PreviousWinnerCannotWinAgain_AndClientCannotSubstituteRecipient()
    {
        var (db, store, organizer, member2, circle) = await SeedCircleWithTwoMembersAsync();
        await store.LockCircleAsync(circle.Id, organizer.Id, CancellationToken.None);

        var round1 = await db.Rounds.FirstAsync(r => r.CircleId == circle.Id && r.RoundNumber == 1);
        foreach (var mId in new[] { organizer.Id, member2.Id })
        {
            var p = await store.AddPaymentAsync(new Payment
            {
                CircleId = circle.Id,
                RoundId = round1.Id,
                MemberId = mId,
                Amount = 1000m,
                BankName = "CBE",
                TransactionId = $"TXN1_{mId}",
                RecordedBy = mId
            }, CancellationToken.None);
            await store.ReviewPaymentAsync(p.Value!.Id, organizer.Id, approve: true, CancellationToken.None);
        }

        // Round 1 recipient is organizer. Try to substitute recipient to member2
        var wrongRecipientRes = await store.AddPayoutAndAdvanceRoundAsync(new Payout
        {
            CircleId = circle.Id,
            RoundId = round1.Id,
            MemberId = member2.Id, // Wrong recipient!
            Amount = 2000m,
            RecordedBy = organizer.Id
        }, CancellationToken.None);

        Assert.False(wrongRecipientRes.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, wrongRecipientRes.StatusCode);
    }

    [Fact]
    public async Task Scenario12_CompletedRound_CannotBePaidOutTwice()
    {
        var (db, store, organizer, member2, circle) = await SeedCircleWithTwoMembersAsync();
        await store.LockCircleAsync(circle.Id, organizer.Id, CancellationToken.None);

        var round1 = await db.Rounds.FirstAsync(r => r.CircleId == circle.Id && r.RoundNumber == 1);
        foreach (var mId in new[] { organizer.Id, member2.Id })
        {
            var p = await store.AddPaymentAsync(new Payment
            {
                CircleId = circle.Id,
                RoundId = round1.Id,
                MemberId = mId,
                Amount = 1000m,
                BankName = "CBE",
                TransactionId = $"TXN1_{mId}",
                RecordedBy = mId
            }, CancellationToken.None);
            await store.ReviewPaymentAsync(p.Value!.Id, organizer.Id, approve: true, CancellationToken.None);
        }

        var payout1 = await store.AddPayoutAndAdvanceRoundAsync(new Payout
        {
            CircleId = circle.Id,
            RoundId = round1.Id,
            MemberId = organizer.Id,
            Amount = 2000m,
            RecordedBy = organizer.Id
        }, CancellationToken.None);
        Assert.True(payout1.IsSuccess);

        // Duplicate payout attempt
        var payout2 = await store.AddPayoutAndAdvanceRoundAsync(new Payout
        {
            CircleId = circle.Id,
            RoundId = round1.Id,
            MemberId = organizer.Id,
            Amount = 2000m,
            RecordedBy = organizer.Id
        }, CancellationToken.None);

        Assert.False(payout2.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, payout2.StatusCode);
    }

    [Fact]
    public async Task Scenario15_UnauthorizedUsers_CannotPerformOrganizerActions()
    {
        var (db, store, organizer, member2, circle) = await SeedCircleWithTwoMembersAsync();

        // Member 2 (not organizer) tries to lock circle
        var lockRes = await store.LockCircleAsync(circle.Id, member2.Id, CancellationToken.None);
        Assert.False(lockRes.IsSuccess);
        Assert.Equal(StatusCodes.Status403Forbidden, lockRes.StatusCode);

        await store.LockCircleAsync(circle.Id, organizer.Id, CancellationToken.None);
        var round1 = await db.Rounds.FirstAsync(r => r.CircleId == circle.Id && r.RoundNumber == 1);

        var payRes = await store.AddPaymentAsync(new Payment
        {
            CircleId = circle.Id,
            RoundId = round1.Id,
            MemberId = member2.Id,
            Amount = 1000m,
            BankName = "CBE",
            TransactionId = "TXN_AUTH_TEST",
            RecordedBy = member2.Id
        }, CancellationToken.None);

        // Member 2 tries to review payment
        var reviewRes = await store.ReviewPaymentAsync(payRes.Value!.Id, member2.Id, approve: true, CancellationToken.None);
        Assert.False(reviewRes.IsSuccess);
        Assert.Equal(StatusCodes.Status403Forbidden, reviewRes.StatusCode);

        // Member 2 tries to record payout
        var payoutRes = await store.AddPayoutAndAdvanceRoundAsync(new Payout
        {
            CircleId = circle.Id,
            RoundId = round1.Id,
            MemberId = organizer.Id,
            Amount = 2000m,
            RecordedBy = member2.Id
        }, CancellationToken.None);
        Assert.False(payoutRes.IsSuccess);
        Assert.Equal(StatusCodes.Status403Forbidden, payoutRes.StatusCode);
    }

    [Fact]
    public async Task Scenario16_PendingOrRejectedPayments_DoNotCountAsApprovedContributions()
    {
        var (db, store, organizer, member2, circle) = await SeedCircleWithTwoMembersAsync();
        await store.LockCircleAsync(circle.Id, organizer.Id, CancellationToken.None);

        var round1 = await db.Rounds.FirstAsync(r => r.CircleId == circle.Id && r.RoundNumber == 1);

        // Both members submit payments
        var p1 = await store.AddPaymentAsync(new Payment
        {
            CircleId = circle.Id,
            RoundId = round1.Id,
            MemberId = organizer.Id,
            Amount = 1000m,
            BankName = "CBE",
            TransactionId = "TXN_ORG",
            RecordedBy = organizer.Id
        }, CancellationToken.None);

        var p2 = await store.AddPaymentAsync(new Payment
        {
            CircleId = circle.Id,
            RoundId = round1.Id,
            MemberId = member2.Id,
            Amount = 1000m,
            BankName = "CBE",
            TransactionId = "TXN_M2",
            RecordedBy = member2.Id
        }, CancellationToken.None);

        // Organizer approves p1, but REJECTS p2
        await store.ReviewPaymentAsync(p1.Value!.Id, organizer.Id, approve: true, CancellationToken.None);
        await store.ReviewPaymentAsync(p2.Value!.Id, organizer.Id, approve: false, CancellationToken.None);

        // Attempt payout - must fail because p2 is rejected
        var payoutRes = await store.AddPayoutAndAdvanceRoundAsync(new Payout
        {
            CircleId = circle.Id,
            RoundId = round1.Id,
            MemberId = organizer.Id,
            Amount = 2000m,
            RecordedBy = organizer.Id
        }, CancellationToken.None);

        Assert.False(payoutRes.IsSuccess);
        Assert.Equal(StatusCodes.Status400BadRequest, payoutRes.StatusCode);
    }
}
