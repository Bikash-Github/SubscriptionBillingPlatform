namespace SubscriptionService.Contracts.Responses;

public record SubscriptionResponse
(
    Guid Id,
    Guid CustomerId,
    string PlanCode,
    string Status,
    DateTime StartDate,
    DateTime? EndDate
);
