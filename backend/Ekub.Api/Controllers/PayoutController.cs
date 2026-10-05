using Asp.Versioning;
using Ekub.Application.Payouts.Commands;
using Ekub.Application.Payouts.DTOs;
using Ekub.Application.Payouts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ekub.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/payouts")]
public sealed class PayoutController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<PayoutResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPayoutsQuery(), cancellationToken);
        return StatusCode(result.StatusCode, result.Value);
    }

    [HttpGet("round/{roundId:int}")]
    [ProducesResponseType(typeof(List<PayoutResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByRoundId(int roundId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPayoutsByRoundQuery(roundId), cancellationToken);
        return StatusCode(result.StatusCode, result.Value);
    }

    [HttpGet("member/{memberId:int}")]
    [ProducesResponseType(typeof(List<PayoutResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByMemberId(int memberId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPayoutsByMemberQuery(memberId), cancellationToken);
        return StatusCode(result.StatusCode, result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PayoutResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePayoutRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreatePayoutCommand(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new { message = result.Error });
        }

        return StatusCode(result.StatusCode, result.Value);
    }
}
