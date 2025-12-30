using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("gateway")]
public class GatewayController : ControllerBase
{

    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public GatewayController(
       IHttpClientFactory clientFactory,
       IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
    }


    [HttpGet("ping")]
    public IActionResult Ping() => Ok("Gateway OK");


    [HttpGet("forward")]
    public async Task<IActionResult> Forward()
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();

        var baseUrl = _configuration["DownstreamServices:SubscriptionService:BaseUrl"];

        var client = _clientFactory.CreateClient("DownstreamClient");

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"{baseUrl}/subscriptions/ping");

        request.Options.Set(
            new HttpRequestOptionsKey<string>("X-Correlation-Id"),
            correlationId!);

        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        return Ok(new
        {
            correlationId,
            downstreamStatusCode = (int)response.StatusCode,
            downstreamResponse = content
        });
    }



}
