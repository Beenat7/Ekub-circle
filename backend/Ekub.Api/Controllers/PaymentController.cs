using Asp.Versioning;
using Ekub.Application.Payments.Commands;
using Ekub.Application.Payments.DTOs;
using Ekub.Application.Payments.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ekub.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/payments")]
public sealed class PaymentController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<PaymentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPaymentsQuery(), cancellationToken);
        return StatusCode(result.StatusCode, result.Value);
    }

    [HttpGet("round/{roundId:int}")]
    [ProducesResponseType(typeof(List<PaymentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByRoundId(int roundId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPaymentsByRoundQuery(roundId), cancellationToken);
        return StatusCode(result.StatusCode, result.Value);
    }

    [HttpGet("member/{memberId:int}")]
    [ProducesResponseType(typeof(List<PaymentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByMemberId(int memberId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPaymentsByMemberQuery(memberId), cancellationToken);
        return StatusCode(result.StatusCode, result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreatePaymentCommand(request), cancellationToken);
        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode, new { message = result.Error });
        }

        return StatusCode(result.StatusCode, result.Value);
    }
}
