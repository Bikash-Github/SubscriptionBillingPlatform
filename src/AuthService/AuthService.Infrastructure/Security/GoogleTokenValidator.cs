using AuthService.Application.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure.Security;

public class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly IConfiguration _configuration;

    public GoogleTokenValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> ValidateAndGetEmailAsync(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _configuration["GoogleAuth:ClientId"]! }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

        return payload.Email; // email is already verified by Google
    }
}