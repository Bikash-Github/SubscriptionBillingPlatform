namespace SubscriptionService.Application.Caching;

public static class CacheKeys
{
    public static string SubscriptionById(Guid id)
        => $"subscription:{id}";
}