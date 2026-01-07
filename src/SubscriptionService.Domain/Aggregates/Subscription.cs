using SubscriptionService.Domain.ValueObjects;

namespace SubscriptionService.Domain.Aggregates;

public class Subscription
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public string PlanCode { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    private Subscription() { } // For persistence

    public Subscription(Guid customerId, string planCode)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        PlanCode = planCode;
        Status = SubscriptionStatus.Active;
        StartDate = DateTime.Now;
    }

    public void Cancel()
    {
        if (Status == SubscriptionStatus.Cancelled)
            throw new InvalidOperationException("Subscription already cancelled");

        Status = SubscriptionStatus.Cancelled;
        EndDate = DateTime.UtcNow;
    }

    public void ChangePlan(string newPlanCode)
    {
        if (Status != SubscriptionStatus.Active)
            throw new InvalidOperationException("Only active subscriptions can change plan");

        if (string.IsNullOrWhiteSpace(newPlanCode))
            throw new ArgumentException("Plan code is required");

        PlanCode = newPlanCode;
    }

}
