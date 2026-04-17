using System.Data;
using System.Data.Common;
using Dapper;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Repositories;

public class LeadRepository(IDbConnection connection) : ILeadRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task<int> InsertAsync(Lead lead)
    {
        await EnsureOpenAsync();
        const string sql = """
            INSERT INTO Leads (ServiceId, Name, Email, Phone, Description, AgreedValue, CreatedAt, Status, InternalNotes)
            VALUES (@ServiceId, @Name, @Email, @Phone, @Description, @Value, @CreatedAtUtc, @Status, @Notes);
            SELECT LAST_INSERT_ID();
            """;
        return await _connection.QuerySingleAsync<int>(sql, lead);
    }

    public async Task<Lead?> GetByIdAsync(int id)
    {
        await EnsureOpenAsync();
        return await _connection.QueryFirstOrDefaultAsync<Lead>(
            """
            SELECT Id, ServiceId, Name, Email, Phone, Description, AgreedValue AS Value, CreatedAt AS CreatedAtUtc, Status, InternalNotes AS Notes
            FROM Leads
            WHERE Id = @Id
            """,
            new { Id = id });
    }

    public async Task<(IReadOnlyList<Lead> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? status, string? search)
    {
        await EnsureOpenAsync();
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);
        var offset = (page - 1) * pageSize;

        var like = string.IsNullOrWhiteSpace(search) ? null : $"%{search.Trim()}%";

        const string whereClause = """
            WHERE (@Status IS NULL OR Status = @Status)
              AND (
                @Like IS NULL
                OR Name LIKE @Like OR Email LIKE @Like OR IFNULL(Phone, '') LIKE @Like
              )
            """;

        var countSql = $"SELECT COUNT(1) FROM Leads {whereClause}";
        var total = await _connection.QuerySingleAsync<int>(
            countSql,
            new { Status = status, Like = like });

        var dataSql = $"""
            SELECT Id, ServiceId, Name, Email, Phone, Description, AgreedValue AS Value, CreatedAt AS CreatedAtUtc, Status, InternalNotes AS Notes
            FROM Leads
            {whereClause}
            ORDER BY CreatedAt DESC
            LIMIT @Take OFFSET @Skip
            """;

        var rows = await _connection.QueryAsync<Lead>(
            dataSql,
            new { Status = status, Like = like, Take = pageSize, Skip = offset });

        return (rows.ToList(), total);
    }

    public async Task<int> UpdateAsync(Lead lead)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            """
            UPDATE Leads SET
                ServiceId = @ServiceId,
                Name = @Name,
                Email = @Email,
                Phone = @Phone,
                Description = @Description,
                AgreedValue = @Value,
                Status = @Status,
                InternalNotes = @Notes
            WHERE Id = @Id
            """,
            lead);
    }

    public async Task<int> UpdateStatusAsync(int id, string status)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            "UPDATE Leads SET Status = @Status WHERE Id = @Id",
            new { Id = id, Status = status });
    }

    public async Task<int> UpdateNotesAsync(int id, string? notes)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            "UPDATE Leads SET InternalNotes = @Notes WHERE Id = @Id",
            new { Id = id, Notes = notes });
    }

    public async Task<int> DeleteAsync(int id)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync("DELETE FROM Leads WHERE Id = @Id", new { Id = id });
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await ((DbConnection)_connection).OpenAsync();
    }
}