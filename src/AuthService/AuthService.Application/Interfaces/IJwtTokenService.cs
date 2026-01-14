using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces;

public interface IJwtTokenService
{   
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}

