using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Circles.DTOs;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;

namespace Ekub.Application.Circles.Queries.GetCircleById;

public sealed record GetCircleByIdQuery(int Id) : IRequest<Result<CircleResponse>>;

public sealed class GetCircleByIdQueryHandler
    : IRequestHandler<GetCircleByIdQuery, Result<CircleResponse>>
{
    private readonly ICircleStore _circleStore;

    public GetCircleByIdQueryHandler(ICircleStore circleStore)
    {
        _circleStore = circleStore;
    }

    public async Task<Result<CircleResponse>> Handle(
        GetCircleByIdQuery query,
        CancellationToken cancellationToken)
    {
        var circle = await _circleStore.GetByIdAsync(query.Id, cancellationToken);

        if (circle is null)
        {
            return Result<CircleResponse>.Failure(
                "Circle not found.",
                StatusCodes.Status404NotFound);
        }

        var response = new CircleResponse(
            circle.Id,
            circle.Name,
            circle.ContributionAmount,
            circle.MaxMembers,
            circle.ContributionIntervalDays,
            circle.Status.ToString(),
            circle.OrganizerId,
            circle.CreatedAt
        );

        return Result<CircleResponse>.Success(response, StatusCodes.Status200OK);
    }
}
