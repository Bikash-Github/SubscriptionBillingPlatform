using SubscriptionService.Contracts;
using SubscriptionService.Contracts.Responses;
using SubscriptionService.Domain.Aggregates;

namespace SubscriptionService.Application.Mappers;

public static class SubscriptionMapper
{
    public static SubscriptionResponse ToResponse(Subscription subscription)
    {
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