namespace Ekub.Application.Payouts.DTOs;

public record PayoutResponse(
    int Id,
    int CircleId,
    int RoundId,
    int MemberId,
    decimal Amount,
    DateTime PaidAt,
    string Status
);

public record CreatePayoutRequest(
    int CircleId,
    int RoundId,
    int MemberId,
    decimal Amount,
    int RecordedBy
);
