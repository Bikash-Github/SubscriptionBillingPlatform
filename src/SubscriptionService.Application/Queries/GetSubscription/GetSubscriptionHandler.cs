using MediatR;
using SubscriptionService.Application.DTOs;
using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Application.Queries.GetSubscription;

public class GetSubscriptionHandler
    : IRequestHandler<GetSubscriptionQuery, SubscriptionDto?>
{
    private readonly ISubscriptionRepository _repository;

    public GetSubscriptionHandler(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<SubscriptionDto?> Handle(
        GetSubscriptionQuery query,
        CancellationToken cancellationToken)
    {
        var subscription = await _repository.GetByIdAsync(query.SubscriptionId);

        if (subscription == null)
            return null;

        return new SubscriptionDto
        {
            Id = subscription.Id,
            CustomerId = subscription.CustomerId,
            PlanCode = subscription.PlanCode,
            Status = subscription.Status.ToString(),
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate
        };
    }
}
