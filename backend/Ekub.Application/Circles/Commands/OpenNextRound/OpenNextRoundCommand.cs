using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;
using Ekub.Application.Rounds.DTOs;

namespace Ekub.Application.Circles.Commands.OpenNextRound;

public sealed record OpenNextRoundRequest(int MemberId);

public sealed record OpenNextRoundCommand(int CircleId, OpenNextRoundRequest Request)
    : IRequest<Result<RoundResponse>>;

public sealed class OpenNextRoundCommandHandler
    : IRequestHandler<OpenNextRoundCommand, Result<RoundResponse>>
{
    private readonly IRoundLifecycleStore _store;

    public OpenNextRoundCommandHandler(IRoundLifecycleStore store)
    {
        _store = store;
    }

    public async Task<Result<RoundResponse>> Handle(
        OpenNextRoundCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _store.OpenNextRoundAsync(
            command.CircleId,
            command.Request.MemberId,
            cancellationToken);
        if (!result.IsSuccess)
        {
            return Result<RoundResponse>.Failure(result.Error!, result.StatusCode);
        }

        var round = result.Value!;
        var response = new RoundResponse(
            round.Id,
            round.CircleId,
            round.Circle.OrganizerId,
            round.RoundNumber,
            round.StartedAt,
            round.DueDate,
            round.ReceiverMemberId,
            round.Status.ToString().ToLowerInvariant(),
            round.Circle.CircleMembers.Count,
            0,
            round.Circle.ContributionAmount,
            round.Circle.ContributionAmount * round.Circle.CircleMembers.Count,
            round.CompletedAt);
        return Result<RoundResponse>.Success(response, StatusCodes.Status201Created);
    }
}
