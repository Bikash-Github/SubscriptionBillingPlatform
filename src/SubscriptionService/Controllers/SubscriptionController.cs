using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.Commands.CreateSubscription;
using SubscriptionService.Application.Commands.CancelSubscription;
using SubscriptionService.Application.Commands.ChangeSubscriptionPlan;
using SubscriptionService.Application.Queries.GetSubscription;

[ApiController]
[Route("subscriptions")]
public class SubscriptionController : ControllerBase
{
    private readonly ILogger<SubscriptionController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;

    public SubscriptionController(
        ILogger<SubscriptionController> logger,
        IHttpClientFactory clientFactory,
        IConfiguration configuration,
        IMediator mediator)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _configuration = configuration;
        _mediator = mediator;
    }

    // -----------------------------
    // CREATE SUBSCRIPTION (COMMAND)
    // -----------------------------
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSubscriptionCommand command)
    {
        _logger.LogInformation(
            "Creating subscription for CustomerId {CustomerId}",
            command.CustomerId);

        var subscriptionId = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id = subscriptionId },
            subscriptionId);
    }

    // -----------------------------
    // GET SUBSCRIPTION (QUERY)
    // -----------------------------
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetSubscriptionQuery(id));

        return result == null ? NotFound() : Ok(result);
    }

    // -----------------------------
    // CANCEL SUBSCRIPTION (COMMAND)
    // -----------------------------
    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        _logger.LogInformation(
            "Cancelling subscription {SubscriptionId}", id);

        await _mediator.Send(new CancelSubscriptionCommand(id));

        return NoContent();
    }

    // -----------------------------
    // CHANGE PLAN (COMMAND)
    // -----------------------------
    [HttpPost("{id}/change-plan")]
    public async Task<IActionResult> ChangePlan(
        Guid id,
        [FromBody] string newPlanCode)
    {
        await _mediator.Send(
            new ChangeSubscriptionPlanCommand(id, newPlanCode));

        return NoContent();
    }

    // -----------------------------
    // PING (INTEGRATION / HEALTH)
    // -----------------------------
    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        _logger.LogInformation("Subscription service ping called");

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
