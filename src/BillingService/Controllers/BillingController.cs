using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("billing")]
public class BillingController : ControllerBase
{
    [HttpPost("charge")]
    public IActionResult Charge() => Ok("Billing processed");
}
