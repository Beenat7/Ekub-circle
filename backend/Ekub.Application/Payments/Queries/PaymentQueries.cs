using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;
using Ekub.Application.Payments.DTOs;

namespace Ekub.Application.Payments.Queries;

public sealed record GetPaymentsQuery() : IRequest<Result<List<PaymentResponse>>>;
public sealed record GetPaymentsByRoundQuery(int RoundId) : IRequest<Result<List<PaymentResponse>>>;
public sealed record GetPaymentsByMemberQuery(int MemberId) : IRequest<Result<List<PaymentResponse>>>;

public sealed class PaymentQueriesHandler :
    IRequestHandler<GetPaymentsQuery, Result<List<PaymentResponse>>>,
    IRequestHandler<GetPaymentsByRoundQuery, Result<List<PaymentResponse>>>,
    IRequestHandler<GetPaymentsByMemberQuery, Result<List<PaymentResponse>>>
{
    private readonly IPaymentStore _store;

    public PaymentQueriesHandler(IPaymentStore store)
    {
        _store = store;
    }

    public async Task<Result<List<PaymentResponse>>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        var payments = await _store.GetAllAsync(cancellationToken);
        var res = payments.Select(p => new PaymentResponse(
            p.Id, p.CircleId, p.RoundId, p.MemberId, p.Amount, p.PaymentMethod.ToString(), p.TransactionId, p.PaidAt, "approved"
        )).ToList();
        return Result<List<PaymentResponse>>.Success(res, StatusCodes.Status200OK);
    }

    public async Task<Result<List<PaymentResponse>>> Handle(GetPaymentsByRoundQuery request, CancellationToken cancellationToken)
    {
        var payments = await _store.GetByRoundIdAsync(request.RoundId, cancellationToken);
        var res = payments.Select(p => new PaymentResponse(
            p.Id, p.CircleId, p.RoundId, p.MemberId, p.Amount, p.PaymentMethod.ToString(), p.TransactionId, p.PaidAt, "approved"
        )).ToList();
        return Result<List<PaymentResponse>>.Success(res, StatusCodes.Status200OK);
    }

    public async Task<Result<List<PaymentResponse>>> Handle(GetPaymentsByMemberQuery request, CancellationToken cancellationToken)
    {
        var payments = await _store.GetByMemberIdAsync(request.MemberId, cancellationToken);
        var res = payments.Select(p => new PaymentResponse(
            p.Id, p.CircleId, p.RoundId, p.MemberId, p.Amount, p.PaymentMethod.ToString(), p.TransactionId, p.PaidAt, "approved"
        )).ToList();
        return Result<List<PaymentResponse>>.Success(res, StatusCodes.Status200OK);
    }
}
