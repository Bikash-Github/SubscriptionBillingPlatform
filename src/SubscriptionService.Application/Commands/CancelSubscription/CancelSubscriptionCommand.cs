using MediatR;

namespace SubscriptionService.Application.Commands.CancelSubscription;

public record CancelSubscriptionCommand(Guid SubscriptionId)
    : IRequest<Unit>;