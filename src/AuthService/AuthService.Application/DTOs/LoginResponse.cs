namespace AuthService.Application.DTOs;

public record LoginResponse(string AccessToken, int ExpiresIn);