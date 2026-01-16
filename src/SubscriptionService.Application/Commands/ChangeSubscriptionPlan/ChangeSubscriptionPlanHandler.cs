using MediatR;
using SubscriptionService.Application.Caching;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Application.Commands.ChangeSubscriptionPlan;

public class ChangeSubscriptionPlanHandler
    : IRequestHandler<ChangeSubscriptionPlanCommand, Unit>
{
    private readonly ISubscriptionRepository _repository;
    private readonly ICacheService _cache;

    public ChangeSubscriptionPlanHandler(ISubscriptionRepository repository,
        ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<Unit> Handle(
        ChangeSubscriptionPlanCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await _repository.GetByIdAsync(command.SubscriptionId);

        if (subscription == null)
            throw new InvalidOperationException("Subscription not found");

        subscription.ChangePlan(command.NewPlanCode);
        await _repository.UpdateAsync(subscription);

        // ✅ invalidate AFTER successful update
        await _cache.RemoveAsync(
            CacheKeys.SubscriptionById(subscription.Id));

        return Unit.Value;
    }
}
