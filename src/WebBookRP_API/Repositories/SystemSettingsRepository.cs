using System.Data;
using Dapper;
using WebBookRP_API.Interfaces;

namespace WebBookRP_API.Repositories;

public class SystemSettingsRepository(IDbConnection connection) : ISystemSettingsRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task<string?> GetValueJsonByKeyAsync(string key)
    {
        await EnsureOpenAsync();
        return await _connection.QueryFirstOrDefaultAsync<string>(
            "SELECT ValueJson FROM system_settings WHERE `Key` = @Key",
            new { Key = key });
    }

    public async Task UpsertAsync(string key, string valueJson)
    {
        await EnsureOpenAsync();
        await _connection.ExecuteAsync(
            """
            INSERT INTO system_settings (`Key`, ValueJson)
            VALUES (@Key, @ValueJson)
            ON DUPLICATE KEY UPDATE ValueJson = VALUES(ValueJson)
            """,
            new { Key = key, ValueJson = valueJson });
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();
    }
}
