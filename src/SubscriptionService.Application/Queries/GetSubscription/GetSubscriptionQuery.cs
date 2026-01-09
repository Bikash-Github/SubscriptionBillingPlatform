using MediatR;
using SubscriptionService.Contracts.Responses;

namespace SubscriptionService.Application.Queries.GetSubscription;

public record GetSubscriptionQuery(Guid SubscriptionId)
    : IRequest<SubscriptionResponse?>;
