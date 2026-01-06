using MediatR;
using SubscriptionService.Domain.Aggregates;
using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Application.Commands.CreateSubscription;

public class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionCommand, Guid>
{
    private readonly ISubscriptionRepository _repository;

    public CreateSubscriptionHandler(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = new Subscription(
            command.CustomerId,
            command.PlanCode);

        await _repository.AddAsync(subscription);

        return subscription.Id;
    }
}
