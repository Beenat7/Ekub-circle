using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Circles.DTOs;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;
using Ekub.Domain.Entities;

namespace Ekub.Application.Circles.Commands.JoinCircle;

public sealed record JoinCircleCommand(
    int CircleId,
    JoinCircleRequest Request
) : IRequest<Result<JoinCircleResponse>>;

public sealed class JoinCircleCommandHandler
    : IRequestHandler<JoinCircleCommand, Result<JoinCircleResponse>>
{
    private readonly ICircleStore _circleStore;

    public JoinCircleCommandHandler(ICircleStore circleStore)
    {
        _circleStore = circleStore;
    }

    public async Task<Result<JoinCircleResponse>> Handle(
        JoinCircleCommand command,
        CancellationToken cancellationToken)
    {
        var circle = await _circleStore.GetByIdWithMembersAsync(command.CircleId, cancellationToken);

        if (circle is null)
        {
            return Result<JoinCircleResponse>.Failure(
                "Circle not found.",
                StatusCodes.Status404NotFound);
        }

        if (circle.Status != CircleStatus.NotStarted)
        {
            return Result<JoinCircleResponse>.Failure(
                "Circle is already in progress or completed and cannot be joined.",
                StatusCodes.Status400BadRequest);
        }

        if (circle.CircleMembers.Count >= circle.MaxMembers)
        {
            return Result<JoinCircleResponse>.Failure(
                "Circle is already full.",
                StatusCodes.Status400BadRequest);
        }

        var isAlreadyMember = await _circleStore.IsMemberInCircleAsync(
            command.CircleId,
            command.Request.MemberId,
            cancellationToken);

        if (isAlreadyMember)
        {
            return Result<JoinCircleResponse>.Failure(
                "User is already a member of this circle.",
                StatusCodes.Status409Conflict);
        }

        var nextOrderNumber = circle.CircleMembers.Count + 1;

        var circleMember = new CircleMember
        {
            CircleId = command.CircleId,
            MemberId = command.Request.MemberId,
            OrderNumber = nextOrderNumber,
            JoinedAt = DateTime.UtcNow
        };

        await _circleStore.AddMemberAsync(circleMember, cancellationToken);

        var response = new JoinCircleResponse(
            circleMember.Id,
            circleMember.CircleId,
            circleMember.MemberId,
            circleMember.OrderNumber,
            circleMember.JoinedAt
        );

        return Result<JoinCircleResponse>.Success(
            response,
            StatusCodes.Status201Created);
    }
}
