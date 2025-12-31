using BuildingBlocks.Infrastructure.Correlation;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Infrastructure.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId =
                context.Request.Headers.TryGetValue(
                    CorrelationHeaders.CorrelationId, out var cid)
                    ? cid.ToString()
                    : Guid.NewGuid().ToString();

            // 🔑 Store for downstream usage (VERY IMPORTANT)
            context.Items[CorrelationHeaders.CorrelationId] = correlationId;

            // Return it to caller
            context.Response.Headers[CorrelationHeaders.CorrelationId] = correlationId;

            // Push into Serilog context
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}
