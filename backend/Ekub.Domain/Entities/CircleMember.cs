using System;

namespace Ekub.Domain.Entities;

public class CircleMember
{
    public int Id { get; set; } // surrogate primary key

    // Foreign key + navigation to the circle
    public int CircleId { get; set; }
    public Circle Circle { get; set; } = null!;

    // Foreign key + navigation to the member
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    // Fixed position in the Ekub rotation order
    public int OrderNumber { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}