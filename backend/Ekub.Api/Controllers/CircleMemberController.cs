using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ekub.Application.CircleMembers.Commands.AddCircleMember;
using Ekub.Application.CircleMembers.DTOs;
using Ekub.Application.CircleMembers.Queries.GetMembersByCircleId;

namespace Ekub.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/circle-members")]
public class CircleMemberController : ControllerBase
{
    private readonly IMediator _mediator;

    public CircleMemberController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CircleMemberResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddMember(
        [FromBody] AddCircleMemberRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddCircleMemberCommand(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new { message = result.Error });
        }

        return StatusCode(result.StatusCode, result.Value);
    }

    [HttpGet("circle/{circleId:int}")]
    [ProducesResponseType(typeof(List<CircleMemberResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCircleId(int circleId, CancellationToken cancellationToken)
    {
        var query = new GetMembersByCircleIdQuery(circleId);
        var result = await _mediator.Send(query, cancellationToken);

        return StatusCode(result.StatusCode, result.Value);
    }
}
