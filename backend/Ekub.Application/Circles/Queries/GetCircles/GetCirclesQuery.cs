using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Circles.DTOs;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;

namespace Ekub.Application.Circles.Queries.GetCircles;

public sealed record GetCirclesQuery() : IRequest<Result<List<CircleResponse>>>;
public sealed record GetCirclesByMemberQuery(int MemberId) : IRequest<Result<List<CircleResponse>>>;

public sealed class GetCirclesQueryHandler
    : IRequestHandler<GetCirclesQuery, Result<List<CircleResponse>>>,
      IRequestHandler<GetCirclesByMemberQuery, Result<List<CircleResponse>>>
{
    private readonly ICircleStore _circleStore;

    public GetCirclesQueryHandler(ICircleStore circleStore)
    {
        _circleStore = circleStore;
    }

    public async Task<Result<List<CircleResponse>>> Handle(
        GetCirclesQuery query,
        CancellationToken cancellationToken)
    {
        var circles = await _circleStore.GetAllAsync(cancellationToken);

        var response = circles.Select(MapResponse).ToList();

        return Result<List<CircleResponse>>.Success(response, StatusCodes.Status200OK);
    }

    public async Task<Result<List<CircleResponse>>> Handle(
        GetCirclesByMemberQuery query,
        CancellationToken cancellationToken)
    {
        var circles = await _circleStore.GetByMemberIdAsync(query.MemberId, cancellationToken);
        var response = circles.Select(MapResponse).ToList();
        return Result<List<CircleResponse>>.Success(response, StatusCodes.Status200OK);
    }

    private static CircleResponse MapResponse(Ekub.Domain.Entities.Circle circle) => new(
        circle.Id,
        circle.Name,
        circle.ContributionAmount,
        circle.MaxMembers,
        circle.ContributionIntervalDays,
        circle.Status.ToString(),
        circle.OrganizerId,
        circle.CreatedAt);
}
