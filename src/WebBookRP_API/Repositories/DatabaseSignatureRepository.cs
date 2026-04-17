using System.Data;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using WebBookRP_API.Interfaces;

namespace WebBookRP_API.Repositories;

public class DatabaseSignatureRepository(IDbConnection connection) : IDatabaseSignatureRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task<string> GetCurrentDatabaseSignatureAsync(CancellationToken ct = default)
    {
        await EnsureOpenAsync(ct);

        var rows = await QuerySchemaRowsAsync(tableNames: null, ct);
        return ComputeSha256Signature(rows);
    }

    public async Task<string> GetSchemaSignatureForTablesAsync(IReadOnlyCollection<string> tableNames, CancellationToken ct = default)
    {
        if (tableNames is null || tableNames.Count == 0)
            return string.Empty;

        await EnsureOpenAsync(ct);
        var rows = await QuerySchemaRowsAsync(tableNames, ct);
        return ComputeSha256Signature(rows);
    }

    private async Task<IEnumerable<SchemaRow>> QuerySchemaRowsAsync(IReadOnlyCollection<string>? tableNames, CancellationToken ct)
    {
        if (tableNames is null)
        {
            return await _connection.QueryAsync<SchemaRow>(
                new CommandDefinition(
                    """
                    SELECT
                        c.TABLE_NAME       AS TableName,
                        c.COLUMN_NAME      AS ColumnName,
                        c.ORDINAL_POSITION AS OrdinalPosition,
                        c.COLUMN_TYPE      AS ColumnType,
                        c.IS_NULLABLE      AS IsNullable,
                        c.COLUMN_DEFAULT   AS ColumnDefault,
                        c.EXTRA            AS Extra,
                        c.COLLATION_NAME   AS CollationName,
                        c.CHARACTER_SET_NAME AS CharacterSetName
                    FROM INFORMATION_SCHEMA.COLUMNS c
                    WHERE c.TABLE_SCHEMA = DATABASE()
                    ORDER BY c.TABLE_NAME, c.ORDINAL_POSITION, c.COLUMN_NAME
                    """,
                    cancellationToken: ct));
        }

        return await _connection.QueryAsync<SchemaRow>(
            new CommandDefinition(
                """
                SELECT
                    c.TABLE_NAME       AS TableName,
                    c.COLUMN_NAME      AS ColumnName,
                    c.ORDINAL_POSITION AS OrdinalPosition,
                    c.COLUMN_TYPE      AS ColumnType,
                    c.IS_NULLABLE      AS IsNullable,
                    c.COLUMN_DEFAULT   AS ColumnDefault,
                    c.EXTRA            AS Extra,
                    c.COLLATION_NAME   AS CollationName,
                    c.CHARACTER_SET_NAME AS CharacterSetName
                FROM INFORMATION_SCHEMA.COLUMNS c
                WHERE c.TABLE_SCHEMA = DATABASE()
                  AND c.TABLE_NAME IN @TableNames
                ORDER BY c.TABLE_NAME, c.ORDINAL_POSITION, c.COLUMN_NAME
                """,
                new { TableNames = tableNames },
                cancellationToken: ct));
    }

    private static string ComputeSha256Signature(IEnumerable<SchemaRow> rows)
    {
        using var sha = SHA256.Create();

        foreach (var r in rows)
        {
            Append(sha, r.TableName);
            Append(sha, "\u001F");
            Append(sha, r.ColumnName);
            Append(sha, "\u001F");
            Append(sha, r.OrdinalPosition.ToString());
            Append(sha, "\u001F");
            Append(sha, r.ColumnType);
            Append(sha, "\u001F");
            Append(sha, r.IsNullable);
            Append(sha, "\u001F");
            Append(sha, r.ColumnDefault ?? "<null>");
            Append(sha, "\u001F");
            Append(sha, r.Extra ?? "<null>");
            Append(sha, "\u001F");
            Append(sha, r.CollationName ?? "<null>");
            Append(sha, "\u001F");
            Append(sha, r.CharacterSetName ?? "<null>");
            Append(sha, "\u001E");
        }

        sha.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        return Convert.ToHexString(sha.Hash!).ToLowerInvariant();
    }

    private static void Append(HashAlgorithm sha, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        sha.TransformBlock(bytes, 0, bytes.Length, null, 0);
    }

    private async Task EnsureOpenAsync(CancellationToken ct)
    {
        if (_connection.State != ConnectionState.Open)
            await ((DbConnection)_connection).OpenAsync(ct);
    }

    private sealed class SchemaRow
    {
        public string TableName { get; init; } = string.Empty;
        public string ColumnName { get; init; } = string.Empty;
        public uint OrdinalPosition { get; init; }
        public string ColumnType { get; init; } = string.Empty;
        public string IsNullable { get; init; } = string.Empty;
        public string? ColumnDefault { get; init; }
        public string? Extra { get; init; }
        public string? CollationName { get; init; }
        public string? CharacterSetName { get; init; }
    }
}

