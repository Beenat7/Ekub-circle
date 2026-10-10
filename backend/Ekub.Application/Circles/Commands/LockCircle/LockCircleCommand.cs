using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Circles.DTOs;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;

namespace Ekub.Application.Circles.Commands.LockCircle;

public sealed record LockCircleCommand(int CircleId, LockCircleRequest Request)
    : IRequest<Result<CircleResponse>>;

public sealed class LockCircleCommandHandler
    : IRequestHandler<LockCircleCommand, Result<CircleResponse>>
{
    private readonly IRoundLifecycleStore _store;

    public LockCircleCommandHandler(IRoundLifecycleStore store)
    {
        _store = store;
    }

    public async Task<Result<CircleResponse>> Handle(
        LockCircleCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _store.LockCircleAsync(
            command.CircleId,
            command.Request.MemberId,
            cancellationToken);
        if (!result.IsSuccess)
        {
            return Result<CircleResponse>.Failure(result.Error!, result.StatusCode);
        }

        var circle = result.Value!;
        return Result<CircleResponse>.Success(
            new CircleResponse(
                circle.Id,
                circle.Name,
                circle.ContributionAmount,
                circle.MaxMembers,
                circle.ContributionIntervalDays,
                circle.Status.ToString(),
                circle.OrganizerId,
                circle.CreatedAt),
            StatusCodes.Status200OK);
    }
}
