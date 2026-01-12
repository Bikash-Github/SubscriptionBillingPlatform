using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenService _jwt;
    private readonly IPasswordHasher _passwordHasher;

    public AuthController(
    IUserRepository users,
    IJwtTokenService jwt,
    IPasswordHasher passwordHasher)
    {
        _users = users;
        _jwt = jwt;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email);

        if (user == null || !user.IsActive)
            return Unauthorized("Invalid credentials");

        // 🔐 Secure verification
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash!))
            return Unauthorized("Invalid credentials");

        var token = _jwt.GenerateToken(user);

        return Ok(new LoginResponse(token, 3600));
    }
}
