using FluentValidation;

namespace SubscriptionService.Application.Commands.ChangeSubscriptionPlan;

public class ChangeSubscriptionPlanCommandValidator
    : AbstractValidator<ChangeSubscriptionPlanCommand>
{
    public ChangeSubscriptionPlanCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty();

        RuleFor(x => x.NewPlanCode)
            .NotEmpty()
            .MaximumLength(50);
    }
}
