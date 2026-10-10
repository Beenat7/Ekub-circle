using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;
using Ekub.Application.Payments.DTOs;

namespace Ekub.Application.Payments.Commands;

public sealed record ReviewPaymentCommand(int PaymentId, ReviewPaymentRequest Request)
    : IRequest<Result<PaymentResponse>>;

public sealed class ReviewPaymentCommandHandler
    : IRequestHandler<ReviewPaymentCommand, Result<PaymentResponse>>
{
    private readonly IRoundLifecycleStore _store;

    public ReviewPaymentCommandHandler(IRoundLifecycleStore store)
    {
        _store = store;
    }

    public async Task<Result<PaymentResponse>> Handle(
        ReviewPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _store.ReviewPaymentAsync(
            command.PaymentId,
            command.Request.ReviewerId,
            command.Request.Approve,
            cancellationToken);
        if (!result.IsSuccess)
        {
            return Result<PaymentResponse>.Failure(result.Error!, result.StatusCode);
        }

        var payment = result.Value!;
        return Result<PaymentResponse>.Success(
            new PaymentResponse(
                payment.Id,
                payment.CircleId,
                payment.RoundId,
                payment.MemberId,
                payment.Amount,
                payment.PaymentMethod.ToString(),
                payment.TransactionId,
                payment.PaidAt,
                payment.Status.ToString().ToLowerInvariant(),
                payment.BankName,
                payment.Circle.Name,
                $"{payment.Member.FirstName} {payment.Member.MiddleName} {payment.Member.LastName}",
                payment.ReviewedByMemberId,
                payment.ReviewedAt));
    }
}
