
namespace AuthService.Infrastructure.Models;
internal sealed class UserRecord
{
    public Guid Id { get; init; }
    public string Email { get; init; } = default!;
    public string Role { get; init; } = default!;
    public string AuthProvider { get; init; } = default!;
    public bool IsActive { get; init; }
    public string? PasswordHash { get; init; }
}