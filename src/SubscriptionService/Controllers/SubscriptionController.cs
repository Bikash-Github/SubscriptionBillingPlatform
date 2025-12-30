using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Domain;

using System;

[ApiController]
[Route("subscriptions")]
public class SubscriptionController : ControllerBase
{

    private readonly ILogger<SubscriptionController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public SubscriptionController(
        ILogger<SubscriptionController> logger,
        IHttpClientFactory clientFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    [HttpPost]
    public IActionResult Create(Guid userId)
    {
        _logger.LogInformation("Creating subscription for UserId {UserId}", userId);

        var sub = new Subscription(userId);

        _logger.LogInformation("Subscription {SubscriptionId} created", sub.Id);

        return Ok(sub);
    }

    [HttpDelete("{id}")]
    public IActionResult Cancel(Guid id)
    {
        return Ok($"Subscription {id} cancelled");
    }

    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        _logger.LogInformation("Subscription service ping called");

        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();

        var billingBaseUrl =
            _configuration["DownstreamServices:BillingService:BaseUrl"];

        var client = _clientFactory.CreateClient("BillingClient");

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{billingBaseUrl}/billing/ping");


        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        return Ok(new
        {
            subscription = "OK",
            billingResponse = content
        });
    }
}
