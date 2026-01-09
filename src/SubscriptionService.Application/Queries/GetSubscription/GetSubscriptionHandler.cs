using MediatR;
using SubscriptionService.Contracts.Responses;
using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Application.Queries.GetSubscription;

public class GetSubscriptionHandler
    : IRequestHandler<GetSubscriptionQuery, SubscriptionResponse?>
{
    private readonly ISubscriptionRepository _repository;

    public GetSubscriptionHandler(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<SubscriptionResponse?> Handle(
        GetSubscriptionQuery query,
        CancellationToken cancellationToken)
    {
        var subscription =
            await _repository.GetByIdAsync(query.SubscriptionId);

        if (subscription == null)
            return null;

        return new SubscriptionResponse(
            subscription.Id,
            subscription.CustomerId,
            subscription.PlanCode,
            subscription.Status.ToString(),
            subscription.StartDate,
            subscription.EndDate
        );
    }
}
