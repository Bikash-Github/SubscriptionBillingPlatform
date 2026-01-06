using MediatR;
using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Application.Commands.CancelSubscription;

public class CancelSubscriptionHandler
    : IRequestHandler<CancelSubscriptionCommand, Unit>
{
    private readonly ISubscriptionRepository _repository;

    public CancelSubscriptionHandler(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(
        CancelSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await _repository.GetByIdAsync(command.SubscriptionId);

        if (subscription == null)
            throw new InvalidOperationException("Subscription not found");

        subscription.Cancel();
        await _repository.UpdateAsync(subscription);

        return Unit.Value;
    }
}
