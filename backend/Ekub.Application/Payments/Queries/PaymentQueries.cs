using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;
using Ekub.Application.Payments.DTOs;

namespace Ekub.Application.Payments.Queries;

public sealed record GetPaymentsQuery() : IRequest<Result<List<PaymentResponse>>>;
public sealed record GetPaymentsByRoundQuery(int RoundId) : IRequest<Result<List<PaymentResponse>>>;
public sealed record GetPaymentsByMemberQuery(int MemberId) : IRequest<Result<List<PaymentResponse>>>;
public sealed record GetPendingPaymentsByOrganizerQuery(int OrganizerId) : IRequest<Result<List<PaymentResponse>>>;

public sealed class PaymentQueriesHandler :
    IRequestHandler<GetPaymentsQuery, Result<List<PaymentResponse>>>,
    IRequestHandler<GetPaymentsByRoundQuery, Result<List<PaymentResponse>>>,
    IRequestHandler<GetPaymentsByMemberQuery, Result<List<PaymentResponse>>>,
    IRequestHandler<GetPendingPaymentsByOrganizerQuery, Result<List<PaymentResponse>>>
{
    private readonly IPaymentStore _store;

    public PaymentQueriesHandler(IPaymentStore store)
    {
        _store = store;
    }

    public async Task<Result<List<PaymentResponse>>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        var payments = await _store.GetAllAsync(cancellationToken);
        var res = payments.Select(MapResponse).ToList();
        return Result<List<PaymentResponse>>.Success(res, StatusCodes.Status200OK);
    }

    public async Task<Result<List<PaymentResponse>>> Handle(GetPaymentsByRoundQuery request, CancellationToken cancellationToken)
    {
        var payments = await _store.GetByRoundIdAsync(request.RoundId, cancellationToken);
        var res = payments.Select(MapResponse).ToList();
        return Result<List<PaymentResponse>>.Success(res, StatusCodes.Status200OK);
    }

    public async Task<Result<List<PaymentResponse>>> Handle(GetPaymentsByMemberQuery request, CancellationToken cancellationToken)
    {
        var payments = await _store.GetByMemberIdAsync(request.MemberId, cancellationToken);
        var res = payments.Select(MapResponse).ToList();
        return Result<List<PaymentResponse>>.Success(res, StatusCodes.Status200OK);
    }

    public async Task<Result<List<PaymentResponse>>> Handle(
        GetPendingPaymentsByOrganizerQuery request,
        CancellationToken cancellationToken)
    {
        var payments = await _store.GetPendingByOrganizerIdAsync(
            request.OrganizerId,
            cancellationToken);
        return Result<List<PaymentResponse>>.Success(
            payments.Select(MapResponse).ToList(),
            StatusCodes.Status200OK);
    }

    private static PaymentResponse MapResponse(Ekub.Domain.Entities.Payment payment) => new(
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
        payment.ReviewedAt);
}
