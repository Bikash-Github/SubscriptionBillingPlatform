namespace SubscriptionService.Contracts.Requests;

public record CreateSubscriptionRequest(
    Guid CustomerId,
    string PlanCode
);
