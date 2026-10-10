using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ekub.Application.Circles.Commands.LockCircle;
using Ekub.Application.Circles.Commands.OpenNextRound;
using Ekub.Application.Circles.Commands.CreateCircle;
using Ekub.Application.Circles.DTOs;
using Ekub.Application.Circles.Queries.GetCircles;

namespace Ekub.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/circles")]
public class CircleController : ControllerBase
{
    private readonly IMediator _mediator;

    public CircleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CircleResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCircles(CancellationToken cancellationToken)
    {
        var query = new GetCirclesQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return StatusCode(result.StatusCode, result.Value);
    }

    [HttpGet("member/{memberId:int}")]
    [ProducesResponseType(typeof(List<CircleResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByMemberId(
        int memberId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetCirclesByMemberQuery(memberId),
            cancellationToken);
        return StatusCode(result.StatusCode, result.Value);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CircleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCircleById(int id, CancellationToken cancellationToken)
    {
        var query = new Ekub.Application.Circles.Queries.GetCircleById.GetCircleByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new { message = result.Error });
        }

        return StatusCode(result.StatusCode, result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CircleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCircleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCircleCommand(request);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return StatusCode(
                result.StatusCode,
                new { message = result.Error });
        }

        return StatusCode(result.StatusCode, result.Value);
    }

    [HttpPatch("{id:int}/lock")]
    [ProducesResponseType(typeof(CircleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Lock(
        int id,
        [FromBody] LockCircleRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new LockCircleCommand(id, request),
            cancellationToken);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new { message = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:int}/rounds/open")]
    [ProducesResponseType(typeof(Ekub.Application.Rounds.DTOs.RoundResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> OpenNextRound(
        int id,
        [FromBody] OpenNextRoundRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new OpenNextRoundCommand(id, request),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new { message = result.Error });
        }

        return CreatedAtRoute(
            "GetRoundById",
            new { version = "1.0", id = result.Value!.Id },
            result.Value);
    }
}
