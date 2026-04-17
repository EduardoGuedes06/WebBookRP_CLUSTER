using System.Data;
using System.Data.Common;
using Dapper;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Repositories;

public class BookRepository(IDbConnection connection) : IBookRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task<IReadOnlyList<Book>> GetAllAsync()
    {
        await EnsureOpenAsync();
        var rows = await _connection.QueryAsync<Book>(
            """
            SELECT Id, Title, Genre, Price, PromoPrice, Active, IsPromotion, Cover, Synopsis, CoverSynopsis,
                   LinkAmazon, LinkML, LinkShopee, LinkGeneric, FullSynopsis, Pages, Rating, Reviews,
                   CreatedAtUtc, UpdatedAtUtc
            FROM books
            ORDER BY CreatedAtUtc DESC
            """);
        return rows.ToList();
    }

    public async Task<IReadOnlyList<Book>> GetActiveCatalogAsync()
    {
        await EnsureOpenAsync();
        var rows = await _connection.QueryAsync<Book>(
            """
            SELECT Id, Title, Genre, Price, PromoPrice, Active, IsPromotion, Cover, Synopsis, CoverSynopsis,
                   LinkAmazon, LinkML, LinkShopee, LinkGeneric, FullSynopsis, Pages, Rating, Reviews,
                   CreatedAtUtc, UpdatedAtUtc
            FROM books
            WHERE Active = 1
            ORDER BY Title
            """);
        return rows.ToList();
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        await EnsureOpenAsync();
        return await _connection.QueryFirstOrDefaultAsync<Book>(
            """
            SELECT Id, Title, Genre, Price, PromoPrice, Active, IsPromotion, Cover, Synopsis, CoverSynopsis,
                   LinkAmazon, LinkML, LinkShopee, LinkGeneric, FullSynopsis, Pages, Rating, Reviews,
                   CreatedAtUtc, UpdatedAtUtc
            FROM books
            WHERE Id = @Id
            """,
            new { Id = id });
    }

    public async Task<int> InsertAsync(Book book)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            """
            INSERT INTO books (
                Id, Title, Genre, Price, PromoPrice, Active, IsPromotion, Cover, Synopsis, CoverSynopsis,
                LinkAmazon, LinkML, LinkShopee, LinkGeneric, FullSynopsis, Pages, Rating, Reviews,
                CreatedAtUtc, UpdatedAtUtc
            ) VALUES (
                @Id, @Title, @Genre, @Price, @PromoPrice, @Active, @IsPromotion, @Cover, @Synopsis, @CoverSynopsis,
                @LinkAmazon, @LinkML, @LinkShopee, @LinkGeneric, @FullSynopsis, @Pages, @Rating, @Reviews,
                @CreatedAtUtc, @UpdatedAtUtc
            )
            """,
            book);
    }

    public async Task<int> UpdateAsync(Book book)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            """
            UPDATE books SET
                Title = @Title,
                Genre = @Genre,
                Price = @Price,
                PromoPrice = @PromoPrice,
                Active = @Active,
                IsPromotion = @IsPromotion,
                Cover = @Cover,
                Synopsis = @Synopsis,
                CoverSynopsis = @CoverSynopsis,
                LinkAmazon = @LinkAmazon,
                LinkML = @LinkML,
                LinkShopee = @LinkShopee,
                LinkGeneric = @LinkGeneric,
                FullSynopsis = @FullSynopsis,
                Pages = @Pages,
                Rating = @Rating,
                Reviews = @Reviews,
                UpdatedAtUtc = @UpdatedAtUtc
            WHERE Id = @Id
            """,
            book);
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync("DELETE FROM books WHERE Id = @Id", new { Id = id });
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await ((DbConnection)_connection).OpenAsync();
    }
}
