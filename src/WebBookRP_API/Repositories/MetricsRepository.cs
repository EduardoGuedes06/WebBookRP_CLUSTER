using Dapper;
using System.Data;
using System.Data.Common;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Repositories;

public class MetricsRepository(IDbConnection connection) : IMetricsRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task RegisterExternalClickAsync(Guid? bookId, string targetStore, string? ipAddress, CancellationToken cancellationToken = default)
    {
        await EnsureOpenAsync();
        const string sql = """
            INSERT INTO ExternalClicks (BookId, TargetStore, IpAddress, CreatedAt)
            VALUES (@BookId, @TargetStore, @IpAddress, @CreatedAt)
            """;

        await _connection.ExecuteAsync(new CommandDefinition(sql, new
        {
            BookId = bookId,
            TargetStore = targetStore,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken: cancellationToken));
    }

    public async Task<int> GetTotalExternalClicksAsync(CancellationToken cancellationToken = default)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteScalarAsync<int>(
            new CommandDefinition("SELECT COUNT(1) FROM ExternalClicks", cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<TrafficSourceAggregate>> GetTrafficSourcesAsync(CancellationToken cancellationToken = default)
    {
        await EnsureOpenAsync();
        const string sql = """
            SELECT TargetStore AS Store, COUNT(1) AS Clicks
            FROM ExternalClicks
            GROUP BY TargetStore
            ORDER BY Clicks DESC
            """;

        var result = await _connection.QueryAsync<TrafficSourceAggregate>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return result.ToList();
    }

    public async Task<LeadFunnelAggregate> GetLeadFunnelAsync(CancellationToken cancellationToken = default)
    {
        await EnsureOpenAsync();
        // Mapeando os status do banco para as propriedades do DTO
        const string sql = """
            SELECT 
                COUNT(CASE WHEN Status = 'pending' THEN 1 END) AS Pending,
                COUNT(CASE WHEN Status = 'in_production' THEN 1 END) AS InProduction,
                COUNT(CASE WHEN Status = 'completed' THEN 1 END) AS Completed
            FROM Leads
            """;

        return await _connection.QuerySingleAsync<LeadFunnelAggregate>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<PopularServiceAggregate>> GetPopularServicesAsync(CancellationToken cancellationToken = default)
    {
        await EnsureOpenAsync();
        const string sql = """
            SELECT s.Name AS ServiceName, COUNT(l.Id) AS LeadCount
            FROM Services s
            INNER JOIN Leads l ON s.Id = l.ServiceId
            GROUP BY s.Id, s.Name
            ORDER BY LeadCount DESC
            LIMIT 5
            """;

        var result = await _connection.QueryAsync<PopularServiceAggregate>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return result.ToList();
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await ((DbConnection)_connection).OpenAsync();
    }
}