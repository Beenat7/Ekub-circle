using System;

namespace Ekub.Domain.Entities;

public class CircleBankAccount
{
    public int Id { get; set; } // surrogate primary key

    public required string BankName { get; set; }

    public required string AccountNumber { get; set; }

    public required string AccountName { get; set; }

    // Foreign key + navigation to the circle
    public int CircleId { get; set; }
    public Circle Circle { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}