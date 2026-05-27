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
            SELECT Id, Title, Genre, Price, PromoPrice, Active, IsPromotion,
                   CoverUrl AS Cover,
                   CAST(NULL AS CHAR) AS Synopsis,
                   CoverSynopsis, FullSynopsis,
                   CAST(NULL AS CHAR) AS LinkAmazon,
                   CAST(NULL AS CHAR) AS LinkML,
                   CAST(NULL AS CHAR) AS LinkShopee,
                   CAST(NULL AS CHAR) AS LinkGeneric,
                   Pages, Rating, ReviewsCount AS Reviews,
                   CreatedAt AS CreatedAtUtc,
                   CreatedAt AS UpdatedAtUtc
            FROM Books
            ORDER BY CreatedAt DESC
            """);
        return rows.ToList();
    }

    public async Task<IReadOnlyList<Book>> GetActiveCatalogAsync()
    {
        await EnsureOpenAsync();
        var rows = await _connection.QueryAsync<Book>(
            """
            SELECT Id, Title, Genre, Price, PromoPrice, Active, IsPromotion,
                   CoverUrl AS Cover,
                   CAST(NULL AS CHAR) AS Synopsis,
                   CoverSynopsis, FullSynopsis,
                   CAST(NULL AS CHAR) AS LinkAmazon,
                   CAST(NULL AS CHAR) AS LinkML,
                   CAST(NULL AS CHAR) AS LinkShopee,
                   CAST(NULL AS CHAR) AS LinkGeneric,
                   Pages, Rating, ReviewsCount AS Reviews,
                   CreatedAt AS CreatedAtUtc,
                   CreatedAt AS UpdatedAtUtc
            FROM Books
            WHERE Active = 1
            ORDER BY Title ASC
            """);
        return rows.ToList();
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        await EnsureOpenAsync();
        return await _connection.QueryFirstOrDefaultAsync<Book>(
            """
            SELECT Id, Title, Genre, Price, PromoPrice, Active, IsPromotion,
                   CoverUrl AS Cover,
                   CoverSynopsis, FullSynopsis,
                   LinkAmazon,
                   LinkML,
                   LinkShopee,
                   LinkGeneric,
                   Pages, Rating, ReviewsCount AS Reviews,
                   CreatedAt AS CreatedAtUtc,
                   UpdatedAtUtc
            FROM Books
            WHERE Id = @Id
            """,
            new { Id = id });
    }

    public async Task<int> InsertAsync(Book book)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            """
            INSERT INTO Books (
                Id, Title, Genre, Price, PromoPrice, Active, IsPromotion, 
                CoverUrl, CoverSynopsis, FullSynopsis, Pages, Rating, ReviewsCount
            ) VALUES (
                @Id, @Title, @Genre, @Price, @PromoPrice, @Active, @IsPromotion, 
                @Cover, @CoverSynopsis, @FullSynopsis, @Pages, @Rating, @Reviews
            )
            """,
            book);
    }

    public async Task<int> UpdateAsync(Book book)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            """
        UPDATE Books SET
            Title = @Title,
            Genre = @Genre,
            Price = @Price,
            PromoPrice = @PromoPrice,
            Active = @Active,
            IsPromotion = @IsPromotion,
            CoverUrl = @Cover, -- Certifique-se de que o objeto 'book' tem a propriedade 'Cover'
            CoverSynopsis = @CoverSynopsis,
            FullSynopsis = @FullSynopsis,
            Pages = @Pages,
            Rating = @Rating,
            ReviewsCount = @Reviews, -- Certifique-se de que o objeto 'book' tem a propriedade 'Reviews'
            LinkAmazon = @LinkAmazon,
            LinkML = @LinkML,
            LinkShopee = @LinkShopee,
            LinkGeneric = @LinkGeneric,
            UpdatedAtUtc = @UpdatedAtUtc
        WHERE Id = @Id
        """,
            book);
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync("DELETE FROM Books WHERE Id = @Id", new { Id = id });
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await ((DbConnection)_connection).OpenAsync();
    }
}