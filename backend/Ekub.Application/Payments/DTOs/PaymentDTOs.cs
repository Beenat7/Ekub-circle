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
    string Status,
    string BankName,
    string? CircleName,
    string? MemberName,
    int? ReviewedByMemberId,
    DateTime? ReviewedAt
);

public record CreatePaymentRequest(
    int CircleId,
    int RoundId,
    int MemberId,
    decimal Amount,
    string PaymentMethod,
    string? TransactionId,
    string? BankName,
    int RecordedBy
);

public sealed record ReviewPaymentRequest(int ReviewerId, bool Approve);
