namespace Ekub.Application.Rounds.DTOs;

public record RoundResponse(
    int Id,
    int CircleId,
    int OrganizerId,
    int RoundNumber,
    DateTime StartDate,
    DateTime EndDate,
    int RecipientId,
    string Status,
    int MemberCount,
    int PaymentsReceived,
    decimal ContributionAmount,
    decimal ExpectedPayoutAmount,
    DateTime? CompletedAt
);
