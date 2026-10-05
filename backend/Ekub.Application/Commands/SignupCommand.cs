using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Ekub.Application.Auth.DTOs;
using Ekub.Application.Common;
using Ekub.Domain.Entities;
using Ekub.Application.Common.Persistence;

namespace Ekub.Application.Auth.Commands.Signup;

public sealed record SignupCommand(
    SignupRequest Request
) : IRequest<Result<SignupResponse>>;

public sealed class SignupCommandHandler
    : IRequestHandler<SignupCommand, Result<SignupResponse>>
{
    private readonly IMemberAuthStore _memberAuthStore;
    private readonly IPasswordHasher<Member> _passwordHasher;

    public SignupCommandHandler(
        IMemberAuthStore memberAuthStore,
        IPasswordHasher<Member> passwordHasher)
    {
        _memberAuthStore = memberAuthStore;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<SignupResponse>> Handle(
        SignupCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        var username = request.Username.Trim();

        var existingMember = await _memberAuthStore.FindByUsernameAsync(
                username,
                cancellationToken);

        if (existingMember is not null)
        {
            return Result<SignupResponse>.Failure(
                "Username is already taken.",
                StatusCodes.Status409Conflict);
        }

        var member = new Member
        {
            Username = username,
            FirstName = request.FirstName.Trim(),
            MiddleName = request.MiddleName.Trim(),
            LastName = request.LastName.Trim(),
            PhoneNumber = request.Phonenumber.Trim(),
            PasswordHash = string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        member.PasswordHash = _passwordHasher.HashPassword(
            member,
            request.Password);

        await _memberAuthStore.AddAsync(member, cancellationToken);

        var response = new SignupResponse(
            member.Id,
            member.Username,
            member.FirstName,
            member.MiddleName,
            member.LastName,
            member.PhoneNumber,
            member.CreatedAt);

        return Result<SignupResponse>.Success(
            response,
            StatusCodes.Status201Created);
    }
}