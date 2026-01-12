namespace AuthService.Domain.Entities;

public class User
{
    public Guid Id { get; }
    public string Email { get; }
    public string Role { get; private set; }
    public string AuthProvider { get; }
    public bool IsActive { get; private set; }
    public string? PasswordHash { get; init; }

    public User(Guid id, string email, string role, string authProvider, string? PasswordHash)
    {
        Id = id;
        Email = email;
        Role = role;
        AuthProvider = authProvider;
        IsActive = true;
        this.PasswordHash = PasswordHash;
    }

    public void Deactivate() => IsActive = false;
    public void ChangeRole(string role) => Role = role;
}