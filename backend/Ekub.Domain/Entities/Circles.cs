using System;

namespace Ekub.Domain.Entities;

public class Circle
{
    public int Id { get; set; } // surrogate primary key

    public required string Name { get; set; }

    public decimal ContributionAmount { get; set; }

    public int MaxMembers { get; set; }

    public int ContributionIntervalDays { get; set; }

    public CircleStatus Status { get; set; } = CircleStatus.NotStarted;

    // The organizer is also a member of the circle
    public int OrganizerId { get; set; }
    public Member Organizer { get; set; } = null!;

    public DateTime? StartedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public ICollection<CircleMember> CircleMembers { get; set; } = new List<CircleMember>();

    public ICollection<CircleBankAccount> BankAccounts { get; set; } = new List<CircleBankAccount>();

    public ICollection<Round> Rounds { get; set; } = new List<Round>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public ICollection<Payout> Payouts { get; set; } = new List<Payout>();
}