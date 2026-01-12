using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Models;
using Dapper;

namespace AuthService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SqlConnectionFactory _factory;

    public UserRepository(SqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = """
            SELECT 
            Id,
            Email,
            Role,
            AuthProvider,
            IsActive,
            PasswordHash
        FROM Users
        WHERE Email = @Email
        """;

        using var connection = _factory.CreateConnection();

        // 🔴 THIS MUST BE UserRecord — NOT User
        var record = await connection
            .QuerySingleOrDefaultAsync<UserRecord>(
                sql,
                new { Email = email });

        if (record == null)
            return null;

        // ✅ Explicit domain creation
        var user = new User(
            record.Id,
            record.Email,
            record.Role,
            record.AuthProvider,
            record.PasswordHash);

        if (!record.IsActive)
            user.Deactivate();

        return user;
    }
}
