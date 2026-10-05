using System;

namespace Ekub.Domain.Entities;

public class Payment
{
    public int Id { get; set; } // surrogate primary key

    // Foreign key + navigation to the circle
    public int CircleId { get; set; }
    public Circle Circle { get; set; } = null!;

    // Foreign key + navigation to the round
    public int RoundId { get; set; }
    public Round Round { get; set; } = null!;

    // Foreign key + navigation to the member
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    public decimal Amount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public string? TransactionId { get; set; }

    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    // Member who recorded/verified the payment
    public int RecordedBy { get; set; }
    public Member RecordedByMember { get; set; } = null!;

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}
