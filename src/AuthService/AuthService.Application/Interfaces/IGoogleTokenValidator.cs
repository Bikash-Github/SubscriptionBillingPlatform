namespace AuthService.Application.Interfaces;

public interface IGoogleTokenValidator
{
    Task<string> ValidateAndGetEmailAsync(string idToken);
}