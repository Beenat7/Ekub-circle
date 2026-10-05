using FluentValidation;

namespace Ekub.Application.Circles.Commands.JoinCircle;

public sealed class JoinCircleCommandValidator : AbstractValidator<JoinCircleCommand>
{
    public JoinCircleCommandValidator()
    {
        RuleFor(x => x.CircleId)
            .GreaterThan(0).WithMessage("Valid Circle ID is required.");

        RuleFor(x => x.Request.MemberId)
            .GreaterThan(0).WithMessage("Valid Member ID is required.");
    }
}
