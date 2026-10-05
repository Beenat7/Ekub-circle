using Asp.Versioning;
using Ekub.Application.Rounds.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ekub.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/rounds")]
public sealed class RoundController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<Ekub.Application.Rounds.DTOs.RoundResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRoundsQuery(), cancellationToken);
        return StatusCode(result.StatusCode, result.Value);
    }

    [HttpGet("circle/{circleId:int}")]
    [ProducesResponseType(typeof(List<Ekub.Application.Rounds.DTOs.RoundResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCircleId(int circleId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRoundsByCircleQuery(circleId), cancellationToken);
        return StatusCode(result.StatusCode, result.Value);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Ekub.Application.Rounds.DTOs.RoundResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRoundByIdQuery(id), cancellationToken);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new { message = result.Error });
        }

        return StatusCode(result.StatusCode, result.Value);
    }
}
