
namespace SubscriptionService.Handlers
{
    public class CorrelationIdDelegatingHandler : DelegatingHandler
    {
        private const string HeaderName = "X-Correlation-Id";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationIdDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        protected override async Task<HttpResponseMessage> SendAsync(
       HttpRequestMessage request,
       CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context != null &&
                context.Items.TryGetValue("CorrelationId", out var correlationIdObj))
            {
                var correlationId = correlationIdObj?.ToString();

                if (!string.IsNullOrWhiteSpace(correlationId) &&
                    !request.Headers.Contains(HeaderName))
                {
                    request.Headers.Add(HeaderName, correlationId);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
