using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using Dapper;

namespace AuthService.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly SqlConnectionFactory _factory;

    public RefreshTokenRepository(SqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task CreateAsync(RefreshToken token)
    {
        const string sql = """
            INSERT INTO RefreshTokens
            (Id, UserId, Token, ExpiresAt, IsRevoked, CreatedAt)
            VALUES
            (@Id, @UserId, @Token, @ExpiresAt, @IsRevoked, GETUTCDATE())
        """;

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, token);
    }

    public async Task<RefreshToken?> GetAsync(string token)
    {
        const string sql = """
            SELECT *
            FROM RefreshTokens
            WHERE Token = @Token
        """;

        using var conn = _factory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<RefreshToken>(sql, new { Token = token });
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        const string sql = """
            UPDATE RefreshTokens
            SET IsRevoked = @IsRevoked,
                ReplacedByToken = @ReplacedByToken
            WHERE Id = @Id
        """;

        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync(sql, token);
    }
}