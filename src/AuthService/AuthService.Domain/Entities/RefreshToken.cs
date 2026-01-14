namespace AuthService.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    // 🔒 Required by Dapper (DO NOT USE DIRECTLY)
    private RefreshToken() { }

    // ✅ Domain constructor (used by application)
    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        IsRevoked = false;
    }

    public void Revoke(string replacedByToken)
    {
        IsRevoked = true;
        ReplacedByToken = replacedByToken;
    }
}