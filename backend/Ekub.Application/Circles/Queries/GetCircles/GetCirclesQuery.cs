using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Circles.DTOs;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;

namespace Ekub.Application.Circles.Queries.GetCircles;

public sealed record GetCirclesQuery() : IRequest<Result<List<CircleResponse>>>;

public sealed class GetCirclesQueryHandler
    : IRequestHandler<GetCirclesQuery, Result<List<CircleResponse>>>
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

        var response = circles.Select(c => new CircleResponse(
            c.Id,
            c.Name,
            c.ContributionAmount,
            c.MaxMembers,
            c.ContributionIntervalDays,
            c.Status.ToString(),
            c.OrganizerId,
            c.CreatedAt
        )).ToList();

        return Result<List<CircleResponse>>.Success(response, StatusCodes.Status200OK);
    }
}
