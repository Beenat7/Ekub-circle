namespace Ekub.Application.Circles.DTOs;

public record CircleResponse(
    int Id,
    string Name,
    decimal ContributionAmount,
    int MaxMembers,
    int ContributionIntervalDays,
    string Status,
    int OrganizerId,
    DateTime CreatedAt
);
