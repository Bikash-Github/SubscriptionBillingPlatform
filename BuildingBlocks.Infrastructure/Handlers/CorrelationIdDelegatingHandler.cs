using BuildingBlocks.Infrastructure.Correlation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Infrastructure.Handlers
{
    public class CorrelationIdDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationIdDelegatingHandler(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context != null &&
                context.Items.TryGetValue(
                    CorrelationHeaders.CorrelationId, out var correlationIdObj))
            {
                var correlationId = correlationIdObj?.ToString();

                if (!string.IsNullOrWhiteSpace(correlationId) &&
                    !request.Headers.Contains(CorrelationHeaders.CorrelationId))
                {
                    request.Headers.Add(
                        CorrelationHeaders.CorrelationId, correlationId);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
