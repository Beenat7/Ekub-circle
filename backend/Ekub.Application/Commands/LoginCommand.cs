using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Ekub.Application.Auth.DTOs;
using Ekub.Application.Common;
using Ekub.Domain.Entities;
using Ekub.Application.Common.Persistence;

namespace Ekub.Application.Auth.Commands.Login;

public sealed record LoginCommand(
    LoginRequest Request
) : IRequest<Result<LoginResponse>>;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IMemberAuthStore _memberAuthStore;
    private readonly IPasswordHasher<Member> _passwordHasher;

    public LoginCommandHandler(
        IMemberAuthStore memberAuthStore,
        IPasswordHasher<Member> passwordHasher)
    {
        _memberAuthStore = memberAuthStore;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<LoginResponse>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        var phoneNumber = request.Phonenumber.Trim();

        var member = await _memberAuthStore.FindByPhoneNumberAsync(
                phoneNumber,
                cancellationToken);

        if (member is null)
        {
            return Result<LoginResponse>.Failure(
                "Invalid phone number or password.",
                StatusCodes.Status401Unauthorized);
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            member,
            member.PasswordHash,
            request.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return Result<LoginResponse>.Failure(
                "Invalid phone number or password.",
                StatusCodes.Status401Unauthorized);
        }

        var response = new LoginResponse(
            member.Id,
            member.Username,
            member.FirstName,
            member.MiddleName,
            member.LastName,
            member.PhoneNumber,
            member.CreatedAt);

        return Result<LoginResponse>.Success(response);
    }
}