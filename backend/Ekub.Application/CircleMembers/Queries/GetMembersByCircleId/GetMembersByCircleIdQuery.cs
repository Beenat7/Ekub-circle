using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.CircleMembers.DTOs;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;
using Ekub.Domain.Entities;

namespace Ekub.Application.CircleMembers.Queries.GetMembersByCircleId;

public sealed record GetMembersByCircleIdQuery(int CircleId) : IRequest<Result<List<CircleMemberResponse>>>;

public sealed class GetMembersByCircleIdQueryHandler
    : IRequestHandler<GetMembersByCircleIdQuery, Result<List<CircleMemberResponse>>>
{
    private readonly ICircleMemberStore _store;

    public GetMembersByCircleIdQueryHandler(ICircleMemberStore store)
    {
        _store = store;
    }

    public async Task<Result<List<CircleMemberResponse>>> Handle(
        GetMembersByCircleIdQuery query,
        CancellationToken cancellationToken)
    {
        var members = await _store.GetByCircleIdAsync(query.CircleId, cancellationToken);
        var response = members.Select(m => new CircleMemberResponse(
            m.Id,
            m.CircleId,
            m.MemberId,
            m.Member != null ? $"{m.Member.FirstName} {m.Member.LastName}".Trim() : $"Member #{m.MemberId}",
            m.OrderNumber,
            m.JoinedAt
        )).ToList();

        return Result<List<CircleMemberResponse>>.Success(response, StatusCodes.Status200OK);
    }
}
