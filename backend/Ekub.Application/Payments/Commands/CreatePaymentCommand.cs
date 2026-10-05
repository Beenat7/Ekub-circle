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
    private readonly IPaymentStore _store;

    public CreatePaymentCommandHandler(IPaymentStore store)
    {
        _store = store;
    }

    public async Task<Result<PaymentResponse>> Handle(CreatePaymentCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        Enum.TryParse<PaymentMethod>(req.PaymentMethod, true, out var method);

        var payment = new Payment
        {
            CircleId = req.CircleId,
            RoundId = req.RoundId,
            MemberId = req.MemberId,
            Amount = req.Amount,
            PaymentMethod = method,
            TransactionId = req.TransactionId,
            PaidAt = DateTime.UtcNow,
            RecordedBy = req.RecordedBy,
            RecordedAt = DateTime.UtcNow
        };

        await _store.AddAsync(payment, cancellationToken);

        var res = new PaymentResponse(
            payment.Id,
            payment.CircleId,
            payment.RoundId,
            payment.MemberId,
            payment.Amount,
            payment.PaymentMethod.ToString(),
            payment.TransactionId,
            payment.PaidAt,
            "approved"
        );

        return Result<PaymentResponse>.Success(res, StatusCodes.Status201Created);
    }
}
