using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;
using Ekub.Application.Rounds.DTOs;

namespace Ekub.Application.Rounds.Queries;

public sealed record GetRoundsQuery() : IRequest<Result<List<RoundResponse>>>;
public sealed record GetRoundsByCircleQuery(int CircleId) : IRequest<Result<List<RoundResponse>>>;
public sealed record GetRoundByIdQuery(int Id) : IRequest<Result<RoundResponse>>;

public sealed class RoundQueriesHandler :
    IRequestHandler<GetRoundsQuery, Result<List<RoundResponse>>>,
    IRequestHandler<GetRoundsByCircleQuery, Result<List<RoundResponse>>>,
    IRequestHandler<GetRoundByIdQuery, Result<RoundResponse>>
{
    private readonly IRoundStore _store;

    public RoundQueriesHandler(IRoundStore store)
    {
        _store = store;
    }

    public async Task<Result<List<RoundResponse>>> Handle(GetRoundsQuery request, CancellationToken cancellationToken)
    {
        var rounds = await _store.GetAllAsync(cancellationToken);
        var res = rounds.Select(r => new RoundResponse(
            r.Id, r.CircleId, r.RoundNumber, r.StartedAt, r.DueDate, r.ReceiverMemberId, r.Status.ToString().ToLower()
        )).ToList();
        return Result<List<RoundResponse>>.Success(res, StatusCodes.Status200OK);
    }

    public async Task<Result<List<RoundResponse>>> Handle(GetRoundsByCircleQuery request, CancellationToken cancellationToken)
    {
        var rounds = await _store.GetByCircleIdAsync(request.CircleId, cancellationToken);
        var res = rounds.Select(r => new RoundResponse(
            r.Id, r.CircleId, r.RoundNumber, r.StartedAt, r.DueDate, r.ReceiverMemberId, r.Status.ToString().ToLower()
        )).ToList();
        return Result<List<RoundResponse>>.Success(res, StatusCodes.Status200OK);
    }

    public async Task<Result<RoundResponse>> Handle(GetRoundByIdQuery request, CancellationToken cancellationToken)
    {
        var r = await _store.GetByIdAsync(request.Id, cancellationToken);
        if (r == null)
        {
            return Result<RoundResponse>.Failure("Round not found.", StatusCodes.Status404NotFound);
        }
        var res = new RoundResponse(
            r.Id, r.CircleId, r.RoundNumber, r.StartedAt, r.DueDate, r.ReceiverMemberId, r.Status.ToString().ToLower()
        );
        return Result<RoundResponse>.Success(res, StatusCodes.Status200OK);
    }
}
