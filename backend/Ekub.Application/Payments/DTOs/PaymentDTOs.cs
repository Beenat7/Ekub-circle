namespace Ekub.Application.Payments.DTOs;

public record PaymentResponse(
    int Id,
    int CircleId,
    int RoundId,
    int MemberId,
    decimal Amount,
    string PaymentMethod,
    string? TransactionId,
    DateTime PaidAt,
    string Status
);

public record CreatePaymentRequest(
    int CircleId,
    int RoundId,
    int MemberId,
    decimal Amount,
    string PaymentMethod,
    string? TransactionId,
    int RecordedBy
);
