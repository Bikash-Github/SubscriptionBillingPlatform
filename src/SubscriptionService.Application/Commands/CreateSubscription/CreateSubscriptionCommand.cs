
using MediatR;

namespace SubscriptionService.Application.Commands.CreateSubscription;

public record CreateSubscriptionCommand(
    Guid CustomerId,
    string PlanCode
) : IRequest<Guid>;
