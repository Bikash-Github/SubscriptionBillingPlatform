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

    public async Task CreateAsync(User user)
    {
        const string sql = """
        INSERT INTO Users
        (Id, Email, PasswordHash, Role, AuthProvider, IsActive, CreatedAt)
        VALUES
        (@Id, @Email, NULL, @Role, @AuthProvider, 1, GETUTCDATE())
    """;

        using var connection = _factory.CreateConnection();

        await connection.ExecuteAsync(sql, new
        {
            user.Id,
            user.Email,
            user.Role,
            user.AuthProvider
        });
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        const string sql = """
        SELECT Id, Email, Role, AuthProvider, IsActive
        FROM Users
        WHERE Id = @Id
    """;

        using var conn = _factory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
    }

}
