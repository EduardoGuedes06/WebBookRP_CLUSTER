using System.Data;
using System.Data.Common;
using Dapper;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Repositories;

public class ServiceRepository(IDbConnection connection) : IServiceRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task<IReadOnlyList<ServiceItem>> GetActiveAsync()
    {
        await EnsureOpenAsync();
        var rows = await _connection.QueryAsync<ServiceItem>(
            """
            SELECT Id, Name, Price, PromoPrice, Unit, Active, IsPromotion, Icon, Theme, Description,
                   CreatedAt AS CreatedAtUtc,
                   CreatedAt AS UpdatedAtUtc
            FROM Services
            WHERE Active = 1
            ORDER BY Name ASC
            """);
        return rows.ToList();
    }

    public async Task<IReadOnlyList<ServiceItem>> GetAllAsync()
    {
        await EnsureOpenAsync();
        var rows = await _connection.QueryAsync<ServiceItem>(
            """
            SELECT Id, Name, Price, PromoPrice, Unit, Active, IsPromotion, Icon, Theme, Description,
                   CreatedAt AS CreatedAtUtc,
                   CreatedAt AS UpdatedAtUtc
            FROM Services
            ORDER BY Name ASC
            """);
        return rows.ToList();
    }

    public async Task<ServiceItem?> GetByIdAsync(Guid id)
    {
        await EnsureOpenAsync();
        return await _connection.QueryFirstOrDefaultAsync<ServiceItem>(
            """
            SELECT Id, Name, Price, PromoPrice, Unit, Active, IsPromotion, Icon, Theme, Description,
                   CreatedAt AS CreatedAtUtc,
                   CreatedAt AS UpdatedAtUtc
            FROM Services
            WHERE Id = @Id
            """,
            new { Id = id });
    }

    public async Task<int> InsertAsync(ServiceItem item)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            """
            INSERT INTO Services (
                Id, Name, Price, PromoPrice, Unit, Active, IsPromotion, Icon, Theme, Description
            ) VALUES (
                @Id, @Name, @Price, @PromoPrice, @Unit, @Active, @IsPromotion, @Icon, @Theme, @Description
            )
            """,
            item);
    }

    public async Task<int> UpdateAsync(ServiceItem item)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            """
            UPDATE Services SET
                Name = @Name,
                Price = @Price,
                PromoPrice = @PromoPrice,
                Unit = @Unit,
                Active = @Active,
                IsPromotion = @IsPromotion,
                Icon = @Icon,
                Theme = @Theme,
                Description = @Description
            WHERE Id = @Id
            """,
            item);
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync("DELETE FROM Services WHERE Id = @Id", new { Id = id });
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await ((DbConnection)_connection).OpenAsync();
    }
}