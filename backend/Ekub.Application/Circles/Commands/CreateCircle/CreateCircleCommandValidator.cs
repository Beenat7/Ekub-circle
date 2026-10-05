using FluentValidation;

namespace Ekub.Application.Circles.Commands.CreateCircle;

public sealed class CreateCircleCommandValidator : AbstractValidator<CreateCircleCommand>
{
    public CreateCircleCommandValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Circle name is required.")
            .MaximumLength(100).WithMessage("Circle name must not exceed 100 characters.");

        RuleFor(x => x.Request.ContributionAmount)
            .GreaterThan(0).WithMessage("Contribution amount must be greater than zero.");

        RuleFor(x => x.Request.MaxMembers)
            .GreaterThan(1).WithMessage("Circle must have at least 2 members.");

        RuleFor(x => x.Request.ContributionIntervalDays)
            .GreaterThan(0).WithMessage("Contribution interval must be at least 1 day.");

        RuleFor(x => x.Request.OrganizerId)
            .GreaterThan(0).WithMessage("Valid Organizer ID is required.");
    }
}
