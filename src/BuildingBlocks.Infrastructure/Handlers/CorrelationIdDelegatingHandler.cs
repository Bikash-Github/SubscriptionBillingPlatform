using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Infrastructure.Handlers;

public class CorrelationIdDelegatingHandler : DelegatingHandler
{
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdDelegatingHandler(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;

        if (context != null &&
            context.Items.TryGetValue("CorrelationId", out var correlationId) &&
            correlationId != null)
        {
            request.Headers.TryAddWithoutValidation(
                CorrelationIdHeader,
                correlationId.ToString());
        }

        return base.SendAsync(request, cancellationToken);
    }
}
