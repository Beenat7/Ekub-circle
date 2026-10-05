using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
}
