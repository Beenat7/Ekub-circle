using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Circles.DTOs;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;

namespace Ekub.Application.Circles.Queries.GetAvailableCircles;

public sealed record GetAvailableCirclesQuery() : IRequest<Result<List<AvailableCircleResponse>>>;

public sealed class GetAvailableCirclesQueryHandler
    : IRequestHandler<GetAvailableCirclesQuery, Result<List<AvailableCircleResponse>>>
{
    private readonly ICircleStore _circleStore;

    public GetAvailableCirclesQueryHandler(ICircleStore circleStore)
    {
        _circleStore = circleStore;
    }

    public async Task<Result<List<AvailableCircleResponse>>> Handle(
        GetAvailableCirclesQuery query,
        CancellationToken cancellationToken)
    {
        var circles = await _circleStore.GetAvailableCirclesAsync(cancellationToken);

        var response = circles.Select(c => new AvailableCircleResponse(
            c.Id,
            c.Name,
            c.ContributionAmount,
            c.MaxMembers,
            c.CircleMembers.Count,
            c.ContributionIntervalDays,
            c.Status.ToString(),
            c.OrganizerId,
            c.Organizer != null ? $"{c.Organizer.FirstName} {c.Organizer.LastName}".Trim() : string.Empty,
            c.CreatedAt
        )).ToList();

        return Result<List<AvailableCircleResponse>>.Success(response, StatusCodes.Status200OK);
    }
}
