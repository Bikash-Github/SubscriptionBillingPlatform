using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("billing")]
public class BillingController : ControllerBase
{

    private readonly ILogger<BillingController> _logger;

    public BillingController(ILogger<BillingController> logger)
    {
        _logger = logger;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        _logger.LogInformation("Billing service ping called");
        return Ok("Billing Service OK");
    }

    [HttpPost("charge")]
    public IActionResult Charge() => Ok("Billing processed");




}
