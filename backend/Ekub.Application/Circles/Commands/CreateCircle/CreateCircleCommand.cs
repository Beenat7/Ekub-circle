using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Circles.DTOs;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;
using Ekub.Domain.Entities;

namespace Ekub.Application.Circles.Commands.CreateCircle;

public sealed record CreateCircleCommand(
    CreateCircleRequest Request
) : IRequest<Result<CircleResponse>>;

public sealed class CreateCircleCommandHandler
    : IRequestHandler<CreateCircleCommand, Result<CircleResponse>>
{
    private readonly ICircleStore _circleStore;

    public CreateCircleCommandHandler(ICircleStore circleStore)
    {
        _circleStore = circleStore;
    }

    public async Task<Result<CircleResponse>> Handle(
        CreateCircleCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;

        var circle = new Circle
        {
            Name = req.Name.Trim(),
            ContributionAmount = req.ContributionAmount,
            MaxMembers = req.MaxMembers,
            ContributionIntervalDays = req.ContributionIntervalDays,
            OrganizerId = req.OrganizerId,
            Status = CircleStatus.NotStarted,
            CreatedAt = DateTime.UtcNow
        };

        await _circleStore.AddAsync(circle, cancellationToken);

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

        return Result<CircleResponse>.Success(
            response,
            StatusCodes.Status201Created);
    }
}
