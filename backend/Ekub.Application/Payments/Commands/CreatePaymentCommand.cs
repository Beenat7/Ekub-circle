using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;
using Ekub.Application.Payments.DTOs;
using Ekub.Domain.Entities;

namespace Ekub.Application.Payments.Commands;

public sealed record CreatePaymentCommand(CreatePaymentRequest Request) : IRequest<Result<PaymentResponse>>;

public sealed class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<PaymentResponse>>
{
    private readonly IRoundLifecycleStore _store;

    public CreatePaymentCommandHandler(IRoundLifecycleStore store)
    {
        _store = store;
    }

    public async Task<Result<PaymentResponse>> Handle(CreatePaymentCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        if (!Enum.TryParse<PaymentMethod>(req.PaymentMethod, true, out var method) ||
            !Enum.IsDefined(method))
        {
            return Result<PaymentResponse>.Failure(
                "Payment method is invalid.",
                StatusCodes.Status400BadRequest);
        }

        if (string.IsNullOrWhiteSpace(req.BankName) ||
            string.IsNullOrWhiteSpace(req.TransactionId))
        {
            return Result<PaymentResponse>.Failure(
                "Bank or provider name and transaction ID are required.",
                StatusCodes.Status400BadRequest);
        }

        if (req.BankName.Trim().Length > 160 ||
            req.TransactionId.Trim().Length > 200)
        {
            return Result<PaymentResponse>.Failure(
                "Bank or provider name must be at most 160 characters and transaction ID at most 200 characters.",
                StatusCodes.Status400BadRequest);
        }

        var payment = new Payment
        {
            CircleId = req.CircleId,
            RoundId = req.RoundId,
            MemberId = req.MemberId,
            Amount = req.Amount,
            PaymentMethod = method,
            BankName = req.BankName.Trim(),
            TransactionId = req.TransactionId.Trim(),
            PaidAt = DateTime.UtcNow,
            RecordedBy = req.RecordedBy,
            RecordedAt = DateTime.UtcNow,
            Status = PaymentStatus.Pending
        };

        var addResult = await _store.AddPaymentAsync(payment, cancellationToken);
        if (!addResult.IsSuccess)
        {
            return Result<PaymentResponse>.Failure(addResult.Error!, addResult.StatusCode);
        }

        var res = new PaymentResponse(
            addResult.Value!.Id,
            addResult.Value.CircleId,
            addResult.Value.RoundId,
            addResult.Value.MemberId,
            addResult.Value.Amount,
            addResult.Value.PaymentMethod.ToString(),
            addResult.Value.TransactionId,
            addResult.Value.PaidAt,
            addResult.Value.Status.ToString().ToLowerInvariant(),
            addResult.Value.BankName,
            addResult.Value.Circle.Name,
            $"{addResult.Value.Member.FirstName} {addResult.Value.Member.MiddleName} {addResult.Value.Member.LastName}",
            addResult.Value.ReviewedByMemberId,
            addResult.Value.ReviewedAt
        );

        return Result<PaymentResponse>.Success(res, StatusCodes.Status201Created);
    }
}
