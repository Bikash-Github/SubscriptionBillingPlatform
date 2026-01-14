namespace AuthService.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string Role { get; private set; } = null!;
    public string AuthProvider { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public string? PasswordHash { get; private set; }

    // 🔒 Required ONLY for Dapper materialization
    private User() { }

    // ✅ Your original domain constructor (unchanged in intent)
    public User(
        Guid id,
        string email,
        string role,
        string authProvider,
        string? passwordHash)
    {
        Id = id;
        Email = email;
        Role = role;
        AuthProvider = authProvider;
        PasswordHash = passwordHash;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;

    public void ChangeRole(string role) => Role = role;
}
