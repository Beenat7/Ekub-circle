using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ekub.Application.Circles.Commands.JoinCircle;
using Ekub.Application.Circles.DTOs;
using Ekub.Application.Circles.Queries.GetAvailableCircles;
using Ekub.Application.Circles.Queries.GetUserCircles;

namespace Ekub.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/circles")]
public class JoinCircleController : ControllerBase
{
    private readonly IMediator _mediator;

    public JoinCircleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns all available circles that members can join.
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(typeof(List<AvailableCircleResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableCircles(CancellationToken cancellationToken)
    {
        var query = new GetAvailableCirclesQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return StatusCode(result.StatusCode, result.Value);
    }

    /// <summary>
    /// Returns all circles joined by a specific member.
    /// </summary>
    [HttpGet("my-circles/{memberId:int}")]
    [ProducesResponseType(typeof(List<AvailableCircleResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserCircles(
        [FromRoute] int memberId,
        CancellationToken cancellationToken)
    {
        var query = new GetUserCirclesQuery(memberId);
        var result = await _mediator.Send(query, cancellationToken);

        return StatusCode(result.StatusCode, result.Value);
    }

    /// <summary>
    /// Allows a user/member to join an available circle.
    /// </summary>
    [HttpPost("{circleId:int}/join")]
    [ProducesResponseType(typeof(JoinCircleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> JoinCircle(
        [FromRoute] int circleId,
        [FromBody] JoinCircleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new JoinCircleCommand(circleId, request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return StatusCode(
                result.StatusCode,
                new { message = result.Error });
        }

        return StatusCode(result.StatusCode, result.Value);
    }
}
