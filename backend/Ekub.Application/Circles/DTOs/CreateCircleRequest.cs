namespace Ekub.Application.Circles.DTOs;

public record CreateCircleRequest(
    string Name,
    decimal ContributionAmount,
    int MaxMembers,
    int ContributionIntervalDays,
    int OrganizerId
);
