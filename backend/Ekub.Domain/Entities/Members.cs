using System;

namespace Ekub.Domain.Entities;

public class Member
{
    public int Id { get; set; } // surrogate primary key

    public required string FirstName { get; set; }

    public required string MiddleName { get; set; }

    public required string LastName { get; set; }

    public required string PhoneNumber { get; set; }

    public required string Username { get; set; }

    public required string PasswordHash { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<CircleMember> CircleMembers { get; set; } = new List<CircleMember>();

    public ICollection<Circle> OrganizedCircles { get; set; } = new List<Circle>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public ICollection<Payout> Payouts { get; set; } = new List<Payout>();
}