using FluentValidation;

namespace SubscriptionService.Application.Commands.CancelSubscription;

public class CancelSubscriptionCommandValidator
    : AbstractValidator<CancelSubscriptionCommand>
{
    public CancelSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty();
    }
}
