namespace Ekub.Application.Circles.DTOs;

public record JoinCircleResponse(
    int CircleMemberId,
    int CircleId,
    int MemberId,
    int OrderNumber,
    DateTime JoinedAt
);
