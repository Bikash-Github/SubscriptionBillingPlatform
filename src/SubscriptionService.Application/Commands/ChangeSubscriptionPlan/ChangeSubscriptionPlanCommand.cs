
using MediatR;
namespace SubscriptionService.Application.Commands.ChangeSubscriptionPlan;

public record ChangeSubscriptionPlanCommand(
    Guid SubscriptionId,
    string NewPlanCode
) : IRequest<Unit>;