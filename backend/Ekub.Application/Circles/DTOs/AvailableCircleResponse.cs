namespace Ekub.Application.Circles.DTOs;

public record AvailableCircleResponse(
    int Id,
    string Name,
    decimal ContributionAmount,
    int MaxMembers,
    int CurrentMemberCount,
    int ContributionIntervalDays,
    string Status,
    int OrganizerId,
    string OrganizerName,
    DateTime CreatedAt
);
