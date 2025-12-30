namespace ApiGateway.Handlers
{
    using System.Net.Http;

    public class CorrelationIdDelegatingHandler : DelegatingHandler
    {
        private const string HeaderName = "X-Correlation-Id";

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(HeaderName) &&
                request.Options.TryGetValue(
                    new HttpRequestOptionsKey<string>(HeaderName),
                    out var correlationId))
            {
                request.Headers.Add(HeaderName, correlationId);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

}
