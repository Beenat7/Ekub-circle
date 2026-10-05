using FluentValidation;

namespace Ekub.Application.Auth.Commands.Login;

public sealed class LoginCommandValidator
    : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Request.Phonenumber)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Request.Password)
            .NotEmpty();
    }
}