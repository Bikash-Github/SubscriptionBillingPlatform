
using MediatR;
using SubscriptionService.Application.DTOs;

namespace SubscriptionService.Application.Queries.GetSubscription;

public record GetSubscriptionQuery(Guid SubscriptionId) : IRequest<SubscriptionDto?>;