using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ekub.Application.Auth.Commands.Login;
using Ekub.Application.Auth.Commands.Signup;
using Ekub.Application.Auth.DTOs;

namespace Ekub.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("signup")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Signup(
        [FromBody] SignupRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SignupCommand(request);

        var result = await _mediator.Send(
            command,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return StatusCode(
                result.StatusCode,
                new
                {
                    message = result.Error
                });
        }

        return StatusCode(
            result.StatusCode,
            result.Value);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request);

        var result = await _mediator.Send(
            command,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return StatusCode(
                result.StatusCode,
                new
                {
                    message = result.Error
                });
        }

        return Ok(result.Value);
    }
}