using SubscriptionService.Domain.Aggregates;

namespace SubscriptionService.Domain.Interfaces;

public interface ISubscriptionRepository
{
    Task AddAsync(Subscription subscription);
    Task<Subscription?> GetByIdAsync(Guid id);
    Task UpdateAsync(Subscription subscription);
}
