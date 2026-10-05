using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;
using Ekub.Application.Payouts.DTOs;

namespace Ekub.Application.Payouts.Queries;

public sealed record GetPayoutsQuery() : IRequest<Result<List<PayoutResponse>>>;
public sealed record GetPayoutsByRoundQuery(int RoundId) : IRequest<Result<List<PayoutResponse>>>;
public sealed record GetPayoutsByMemberQuery(int MemberId) : IRequest<Result<List<PayoutResponse>>>;

public sealed class PayoutQueriesHandler :
    IRequestHandler<GetPayoutsQuery, Result<List<PayoutResponse>>>,
    IRequestHandler<GetPayoutsByRoundQuery, Result<List<PayoutResponse>>>,
    IRequestHandler<GetPayoutsByMemberQuery, Result<List<PayoutResponse>>>
{
    private readonly IPayoutStore _store;

    public PayoutQueriesHandler(IPayoutStore store)
    {
        _store = store;
    }

    public async Task<Result<List<PayoutResponse>>> Handle(GetPayoutsQuery request, CancellationToken cancellationToken)
    {
        var payouts = await _store.GetAllAsync(cancellationToken);
        var res = payouts.Select(p => new PayoutResponse(
            p.Id, p.CircleId, p.RoundId, p.MemberId, p.Amount, p.PaidAt, "completed"
        )).ToList();
        return Result<List<PayoutResponse>>.Success(res, StatusCodes.Status200OK);
    }

    public async Task<Result<List<PayoutResponse>>> Handle(GetPayoutsByRoundQuery request, CancellationToken cancellationToken)
    {
        var payouts = await _store.GetByRoundIdAsync(request.RoundId, cancellationToken);
        var res = payouts.Select(p => new PayoutResponse(
            p.Id, p.CircleId, p.RoundId, p.MemberId, p.Amount, p.PaidAt, "completed"
        )).ToList();
        return Result<List<PayoutResponse>>.Success(res, StatusCodes.Status200OK);
    }

    public async Task<Result<List<PayoutResponse>>> Handle(GetPayoutsByMemberQuery request, CancellationToken cancellationToken)
    {
        var payouts = await _store.GetByMemberIdAsync(request.MemberId, cancellationToken);
        var res = payouts.Select(p => new PayoutResponse(
            p.Id, p.CircleId, p.RoundId, p.MemberId, p.Amount, p.PaidAt, "completed"
        )).ToList();
        return Result<List<PayoutResponse>>.Success(res, StatusCodes.Status200OK);
    }
}
