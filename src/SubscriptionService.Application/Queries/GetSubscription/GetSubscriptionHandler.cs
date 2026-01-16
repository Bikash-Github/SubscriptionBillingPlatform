using MediatR;
using SubscriptionService.Application.Caching;
using SubscriptionService.Application.Interfaces;
using SubscriptionService.Application.Mappers;
using SubscriptionService.Contracts.Responses;
using SubscriptionService.Domain.Interfaces;

namespace SubscriptionService.Application.Queries.GetSubscription;

public class GetSubscriptionHandler
    : IRequestHandler<GetSubscriptionQuery, SubscriptionResponse?>
{
    private readonly ISubscriptionRepository _repository;
    private readonly ICacheService _cache;

    public GetSubscriptionHandler(
        ISubscriptionRepository repository,
        ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<SubscriptionResponse?> Handle(
        GetSubscriptionQuery request,
        CancellationToken cancellationToken)
    {
        
        var cacheKey = CacheKeys.SubscriptionById(request.SubscriptionId);

        var cached = await _cache.GetAsync<SubscriptionResponse>(cacheKey);
        if (cached != null)
            return cached;

        var subscription =
            await _repository.GetByIdAsync(request.SubscriptionId);

        if (subscription == null)
            return null;

        var response = SubscriptionMapper.ToResponse(subscription);

        await _cache.SetAsync(
            cacheKey,
            response,
            TimeSpan.FromMinutes(5));

        return response;
    }
}
