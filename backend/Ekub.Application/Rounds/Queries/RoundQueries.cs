using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;
using Ekub.Application.Rounds.DTOs;
using Ekub.Domain.Entities;

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
        var res = rounds.Select(MapResponse).ToList();
        return Result<List<RoundResponse>>.Success(res, StatusCodes.Status200OK);
    }

    public async Task<Result<List<RoundResponse>>> Handle(GetRoundsByCircleQuery request, CancellationToken cancellationToken)
    {
        var rounds = await _store.GetByCircleIdAsync(request.CircleId, cancellationToken);
        var res = rounds.Select(MapResponse).ToList();
        return Result<List<RoundResponse>>.Success(res, StatusCodes.Status200OK);
    }

    public async Task<Result<RoundResponse>> Handle(GetRoundByIdQuery request, CancellationToken cancellationToken)
    {
        var r = await _store.GetByIdAsync(request.Id, cancellationToken);
        if (r == null)
        {
            return Result<RoundResponse>.Failure("Round not found.", StatusCodes.Status404NotFound);
        }
        var res = MapResponse(r);
        return Result<RoundResponse>.Success(res, StatusCodes.Status200OK);
    }

    private static RoundResponse MapResponse(Round round) => new(
        round.Id,
        round.CircleId,
        round.Circle.OrganizerId,
        round.RoundNumber,
        round.StartedAt,
        round.DueDate,
        round.ReceiverMemberId,
        round.Status.ToString().ToLowerInvariant(),
        round.Circle.CircleMembers.Count,
        round.Payments
            .Where(payment => payment.Status == PaymentStatus.Approved)
            .Select(payment => payment.MemberId)
            .Distinct()
            .Count(),
        round.Circle.ContributionAmount,
        round.Circle.ContributionAmount * round.Circle.CircleMembers.Count,
        round.CompletedAt
    );
}
