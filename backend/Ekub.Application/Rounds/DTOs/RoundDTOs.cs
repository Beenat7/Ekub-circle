namespace Ekub.Application.Rounds.DTOs;

public record RoundResponse(
    int Id,
    int CircleId,
    int RoundNumber,
    DateTime StartDate,
    DateTime EndDate,
    int RecipientId,
    string Status
);
