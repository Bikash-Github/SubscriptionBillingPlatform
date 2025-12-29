using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("gateway")]
public class GatewayController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Ok("Gateway OK");
}
