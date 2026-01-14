namespace AuthService.API.Responses;

public record LoginResponse(string AccessToken, string RefreshToken, int ExpiresIn);