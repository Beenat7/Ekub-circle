using System;

namespace Ekub.Domain.Entities;

public class Round
{
    public int Id { get; set; } // surrogate primary key

    public int RoundNumber { get; set; }

    // Foreign key + navigation to the circle
    public int CircleId { get; set; }
    public Circle Circle { get; set; } = null!;

    // The member entitled to receive the pot for this round
    public int ReceiverMemberId { get; set; }
    public Member ReceiverMember { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public RoundStatus Status { get; set; } = RoundStatus.Pending;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public Payout? Payout { get; set; }
}