
namespace ApiGateway.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using SubscriptionService.Contracts.Requests;
    using System.Text;
    using System.Text.Json;

    [ApiController]
    [Route("gateway/subscriptions")]
    public class SubscriptionGatewayController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SubscriptionGatewayController> _logger;

        public SubscriptionGatewayController(
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            ILogger<SubscriptionGatewayController> logger)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        // ----------------------------------
        // CREATE SUBSCRIPTION (FORWARD)
        // ----------------------------------
        [HttpPost]
        public async Task<IActionResult> CreateSubscription(
    [FromBody] CreateSubscriptionRequest request)
        {
            var baseUrl =
                _configuration["DownstreamServices:SubscriptionService:BaseUrl"];

            var client = _clientFactory.CreateClient("DownstreamClient");

            var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"{baseUrl}/subscriptions")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json")
            };

            var response = await client.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, content);
        }

        // ----------------------------------
        // GET SUBSCRIPTION (FORWARD)
        // ----------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubscription(Guid id)
        {
            var baseUrl =
                _configuration["DownstreamServices:SubscriptionService:BaseUrl"];

            var client = _clientFactory.CreateClient("DownstreamClient");

            var response = await client.GetAsync(
                $"{baseUrl}/subscriptions/{id}");

            var content = await response.Content.ReadAsStringAsync();

            return StatusCode(
                (int)response.StatusCode,
                content);
        }

        // ----------------------------------
        // CANCEL SUBSCRIPTION (FORWARD)
        // ----------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelSubscription(Guid id)
        {
            var baseUrl =
                _configuration["DownstreamServices:SubscriptionService:BaseUrl"];

            var client = _clientFactory.CreateClient("DownstreamClient");

            var response = await client.DeleteAsync(
                $"{baseUrl}/subscriptions/{id}");

            return StatusCode((int)response.StatusCode);
        }

        // ----------------------------------
        // CHANGE SUBSCRIPTION PLAN (FORWARD)
        // ----------------------------------
        [HttpPost("{id}/change-plan")]
        public async Task<IActionResult> ChangeSubscriptionPlan(
            Guid id,
            [FromBody] string newPlanCode)
        {
            var baseUrl =
                _configuration["DownstreamServices:SubscriptionService:BaseUrl"];

            var client = _clientFactory.CreateClient("DownstreamClient");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{baseUrl}/subscriptions/{id}/change-plan")
            {
                Content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(newPlanCode),
                    Encoding.UTF8,
                    "application/json")
            };

            _logger.LogInformation(
                "Forwarding ChangeSubscriptionPlan for SubscriptionId {SubscriptionId}",
                id);

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            return StatusCode(
                (int)response.StatusCode,
                content);
        }

    }

}
