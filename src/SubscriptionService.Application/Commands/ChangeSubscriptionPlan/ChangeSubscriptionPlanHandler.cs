using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Application.Commands.ChangeSubscriptionPlan;

public class ChangeSubscriptionPlanHandler
{
    private readonly ISubscriptionRepository _repository;

    public ChangeSubscriptionPlanHandler(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(ChangeSubscriptionPlanCommand command)
    {
        var subscription = await _repository.GetByIdAsync(command.SubscriptionId);

        if (subscription == null)
            throw new InvalidOperationException("Subscription not found");

        subscription.ChangePlan(command.NewPlanCode);

        await _repository.UpdateAsync(subscription);
    }
}
