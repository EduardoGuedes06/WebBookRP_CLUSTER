using System.Data;
using System.Data.Common;
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
            """
            SELECT CAST(ConfigValue AS CHAR)
            FROM SystemSettings
            WHERE ConfigKey = @Key
            """,
            new { Key = key });
    }

    public async Task UpsertAsync(string key, string valueJson)
    {
        await EnsureOpenAsync();
        await _connection.ExecuteAsync(
            """
            INSERT INTO SystemSettings (ConfigKey, ConfigValue)
            VALUES (@Key, CAST(@ValueJson AS JSON))
            ON DUPLICATE KEY UPDATE ConfigValue = CAST(@ValueJson AS JSON)
            """,
            new { Key = key, ValueJson = valueJson });
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await ((DbConnection)_connection).OpenAsync();
    }
}