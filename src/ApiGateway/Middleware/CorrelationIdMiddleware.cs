using Serilog.Context;

namespace ApiGateway.Middleware
{
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //Try to get the correlation ID from the incoming request headers, if not present generate a new one.
            var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var existing)
                ? existing.ToString()
                : Guid.NewGuid().ToString();

            //set the correlation ID in the response headers for client reference
            context.Response.Headers[HeaderName] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}
