using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Application.Commands.CancelSubscription;
using SubscriptionService.Application.Commands.ChangeSubscriptionPlan;
using SubscriptionService.Application.Commands.CreateSubscription;
using SubscriptionService.Application.Queries.GetSubscription;
using SubscriptionService.Contracts.Requests;

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
    [FromBody] CreateSubscriptionRequest request)
    {
        // 1️. Create command
        var command = new CreateSubscriptionCommand(
            request.CustomerId,
            request.PlanCode);

        // 2️. Execute command → get Id
        var id = await _mediator.Send(command);

        // 3️. Query full state
        var response =
            await _mediator.Send(new GetSubscriptionQuery(id));

        if (response == null)
            return StatusCode(500, "Subscription created but not found");

        // 4️. Return contract response
        return CreatedAtAction(
            nameof(GetById),
            new { id },
            response);
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
