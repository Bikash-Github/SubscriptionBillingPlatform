using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenService _jwt;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    public AuthController(
    IUserRepository users,
    IJwtTokenService jwt,
    IPasswordHasher passwordHasher,
    IRefreshTokenRepository refreshTokenRepo)
    {
        _users = users;
        _jwt = jwt;
        _passwordHasher = passwordHasher;
        _refreshTokenRepo = refreshTokenRepo;
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
                
        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshTokenValue = _jwt.GenerateRefreshToken();

        var refreshToken = new RefreshToken(
        user.Id,
        refreshTokenValue,
        DateTime.UtcNow.AddDays(14));

        await _refreshTokenRepo.CreateAsync(refreshToken);

        return Ok(new LoginResponse(
        accessToken,
        refreshTokenValue,
        1800));
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin(
    GoogleLoginRequest request,
    [FromServices] IGoogleTokenValidator googleValidator)
    {
        var email = await googleValidator.ValidateAndGetEmailAsync(request.IdToken);

        var user = await _users.GetByEmailAsync(email);

        if (user == null)
        {
            //No password required if the user signin through Google.
            user = new User(
                Guid.NewGuid(),
                email,
                role: "User",
                authProvider: "Google",
                passwordHash: "");

            await _users.CreateAsync(user);
        }

        if (!user.IsActive)
            return Unauthorized("User is disabled");

        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshTokenValue = _jwt.GenerateRefreshToken();

        var refreshToken = new RefreshToken(
            user.Id,
            refreshTokenValue,
            DateTime.UtcNow.AddDays(14));

        await _refreshTokenRepo.CreateAsync(refreshToken);

        return Ok(new LoginResponse(
            accessToken,
            refreshTokenValue,
            1800));
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var existing = await _refreshTokenRepo.GetAsync(request.RefreshToken);

        if (existing == null || existing.IsRevoked || existing.ExpiresAt < DateTime.UtcNow)
            return Unauthorized("Invalid refresh token");

        var user = await _users.GetByIdAsync(existing.UserId);

        if (user == null || !user.IsActive)
            return Unauthorized();

        var newAccessToken = _jwt.GenerateAccessToken(user);
        var newRefreshTokenValue = _jwt.GenerateRefreshToken();

        existing.Revoke(newRefreshTokenValue);

        await _refreshTokenRepo.UpdateAsync(existing);

        var newRefreshToken = new RefreshToken(
            user.Id,
            newRefreshTokenValue,
            DateTime.UtcNow.AddDays(14));

        await _refreshTokenRepo.CreateAsync(newRefreshToken);

        return Ok(new LoginResponse(
            newAccessToken,
            newRefreshTokenValue,
            1800));
    }


}
