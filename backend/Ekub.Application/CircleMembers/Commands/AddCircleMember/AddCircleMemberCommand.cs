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
    private readonly ICircleMemberStore _store;

    public AddCircleMemberCommandHandler(ICircleMemberStore store)
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

        await _store.AddAsync(cm, cancellationToken);

        var response = new CircleMemberResponse(
            cm.Id,
            cm.CircleId,
            cm.MemberId,
            $"Member #{cm.MemberId}",
            cm.OrderNumber,
            cm.JoinedAt
        );

        return Result<CircleMemberResponse>.Success(response, StatusCodes.Status201Created);
    }
}
