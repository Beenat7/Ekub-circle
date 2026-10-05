namespace Ekub.Application.CircleMembers.DTOs;

public record CircleMemberResponse(
    int Id,
    int CircleId,
    int MemberId,
    string MemberName,
    int OrderNumber,
    DateTime JoinedAt
);

public record AddCircleMemberRequest(
    int CircleId,
    int MemberId,
    int OrderNumber
);
