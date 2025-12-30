namespace BillingService.Middleware
{
    using Serilog.Context;

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
            Console.WriteLine(
    $"[BillingService] Incoming X-Correlation-Id header: {context.Request.Headers["X-Correlation-Id"]}");

            var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var cid)
                ? cid.ToString()
                : Guid.NewGuid().ToString();

            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers[HeaderName] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }

}
