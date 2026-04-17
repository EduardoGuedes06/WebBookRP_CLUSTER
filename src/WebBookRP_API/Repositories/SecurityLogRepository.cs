using System.Data;
using System.Data.Common;
using Dapper;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Repositories;

public class SecurityLogRepository(IDbConnection connection) : ISecurityLogRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task<int> InsertAsync(SecurityLog log)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            """
            INSERT INTO security_logs (EmailAttempt, Success, IpAddress, UserAgent, CreatedAtUtc)
            VALUES (@EmailAttempt, @Success, @IpAddress, @UserAgent, @CreatedAtUtc)
            """,
            log);
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await ((DbConnection)_connection).OpenAsync();
    }
}
