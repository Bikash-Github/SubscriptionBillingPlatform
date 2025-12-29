using System;

namespace SubscriptionService.Domain;

public class Subscription
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public bool IsActive { get; private set; }

    public Subscription(Guid userId)
    {
        UserId = userId;
        IsActive = true;
    }

    public void Cancel() => IsActive = false;
}
