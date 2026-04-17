using System.Data;
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
            SELECT Id, Name, Price, Unit, Active, IsPromotion, PromoPrice, Icon, Theme, Description,
                   CreatedAtUtc, UpdatedAtUtc
            FROM services
            WHERE Active = 1
            ORDER BY Name
            """);
        return rows.ToList();
    }

    public async Task<IReadOnlyList<ServiceItem>> GetAllAsync()
    {
        await EnsureOpenAsync();
        var rows = await _connection.QueryAsync<ServiceItem>(
            """
            SELECT Id, Name, Price, Unit, Active, IsPromotion, PromoPrice, Icon, Theme, Description,
                   CreatedAtUtc, UpdatedAtUtc
            FROM services
            ORDER BY Name
            """);
        return rows.ToList();
    }

    public async Task<ServiceItem?> GetByIdAsync(Guid id)
    {
        await EnsureOpenAsync();
        return await _connection.QueryFirstOrDefaultAsync<ServiceItem>(
            """
            SELECT Id, Name, Price, Unit, Active, IsPromotion, PromoPrice, Icon, Theme, Description,
                   CreatedAtUtc, UpdatedAtUtc
            FROM services
            WHERE Id = @Id
            """,
            new { Id = id });
    }

    public async Task<int> InsertAsync(ServiceItem item)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            """
            INSERT INTO services (
                Id, Name, Price, Unit, Active, IsPromotion, PromoPrice, Icon, Theme, Description,
                CreatedAtUtc, UpdatedAtUtc
            ) VALUES (
                @Id, @Name, @Price, @Unit, @Active, @IsPromotion, @PromoPrice, @Icon, @Theme, @Description,
                @CreatedAtUtc, @UpdatedAtUtc
            )
            """,
            item);
    }

    public async Task<int> UpdateAsync(ServiceItem item)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            """
            UPDATE services SET
                Name = @Name,
                Price = @Price,
                Unit = @Unit,
                Active = @Active,
                IsPromotion = @IsPromotion,
                PromoPrice = @PromoPrice,
                Icon = @Icon,
                Theme = @Theme,
                Description = @Description,
                UpdatedAtUtc = @UpdatedAtUtc
            WHERE Id = @Id
            """,
            item);
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync("DELETE FROM services WHERE Id = @Id", new { Id = id });
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();
    }
}
