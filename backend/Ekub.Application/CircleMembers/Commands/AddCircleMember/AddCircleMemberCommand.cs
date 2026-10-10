using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.CircleMembers.DTOs;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;
using Ekub.Domain.Entities;

namespace Ekub.Application.CircleMembers.Commands.AddCircleMember;

public sealed record AddCircleMemberCommand(AddCircleMemberRequest Request) : IRequest<Result<CircleMemberResponse>>;

public sealed class AddCircleMemberCommandHandler
    : IRequestHandler<AddCircleMemberCommand, Result<CircleMemberResponse>>
{
    private readonly IRoundLifecycleStore _store;

    public AddCircleMemberCommandHandler(IRoundLifecycleStore store)
    {
        _store = store;
    }

    public async Task<Result<CircleMemberResponse>> Handle(
        AddCircleMemberCommand command,
        CancellationToken cancellationToken)
    {
        var req = command.Request;
        var cm = new CircleMember
        {
            CircleId = req.CircleId,
            MemberId = req.MemberId,
            OrderNumber = req.OrderNumber,
            JoinedAt = DateTime.UtcNow
        };

        var addResult = await _store.AddCircleMemberAsync(cm, cancellationToken);
        if (!addResult.IsSuccess)
        {
            return Result<CircleMemberResponse>.Failure(addResult.Error!, addResult.StatusCode);
        }

        var response = new CircleMemberResponse(
            addResult.Value!.Id,
            addResult.Value.CircleId,
            addResult.Value.MemberId,
            $"Member #{addResult.Value.MemberId}",
            addResult.Value.OrderNumber,
            addResult.Value.JoinedAt
        );

        return Result<CircleMemberResponse>.Success(response, StatusCodes.Status201Created);
    }
}
