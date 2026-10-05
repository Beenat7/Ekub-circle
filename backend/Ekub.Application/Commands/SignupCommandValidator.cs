using FluentValidation;

namespace Ekub.Application.Auth.Commands.Signup;

public sealed class SignupCommandValidator
    : AbstractValidator<SignupCommand>
{
    public SignupCommandValidator()
    {
        RuleFor(x => x.Request.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);

        RuleFor(x => x.Request.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Request.MiddleName)
            .MaximumLength(100);

        RuleFor(x => x.Request.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Request.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100);
    }
}