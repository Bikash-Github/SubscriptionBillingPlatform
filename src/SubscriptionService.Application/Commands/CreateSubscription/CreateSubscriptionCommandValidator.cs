using FluentValidation;

namespace SubscriptionService.Application.Commands.CreateSubscription;

public class CreateSubscriptionCommandValidator
    : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("CustomerId is required");

        RuleFor(x => x.PlanCode)
            .NotEmpty()
            .WithMessage("PlanCode is required")
            .MaximumLength(50);
    }
}
