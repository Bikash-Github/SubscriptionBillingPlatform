using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Domain;

using System;

[ApiController]
[Route("subscriptions")]
public class SubscriptionController : ControllerBase
{

    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(ILogger<SubscriptionController> logger)
    {
        _logger = logger;
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
}
