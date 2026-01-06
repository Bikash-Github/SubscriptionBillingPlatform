using Dapper;
using SubscriptionService.Domain.Aggregates;
using SubscriptionService.Domain.Interfaces;
using SubscriptionService.Infrastructure.Persistence;
using System.Data;

namespace SubscriptionService.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public SubscriptionRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(Subscription subscription)
    {
        const string sql = @"
            INSERT INTO Subscriptions
            (Id, CustomerId, PlanCode, Status, StartDate, EndDate)
            VALUES
            (@Id, @CustomerId, @PlanCode, @Status, @StartDate, @EndDate)";

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new
        {
            subscription.Id,
            subscription.CustomerId,
            subscription.PlanCode,
            Status = (int)subscription.Status,
            subscription.StartDate,
            subscription.EndDate
        });
    }

    public async Task<Subscription?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT *
            FROM Subscriptions
            WHERE Id = @Id";

        using var connection = _connectionFactory.Create();

        return await connection.QuerySingleOrDefaultAsync<Subscription>(
            sql, new { Id = id });
    }

    public async Task UpdateAsync(Subscription subscription)
    {
        const string sql = @"
            UPDATE Subscriptions
            SET
                PlanCode = @PlanCode,
                Status = @Status,
                EndDate = @EndDate
            WHERE Id = @Id";

        using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(sql, new
        {
            subscription.Id,
            subscription.PlanCode,
            Status = (int)subscription.Status,
            subscription.EndDate
        });
    }
}
