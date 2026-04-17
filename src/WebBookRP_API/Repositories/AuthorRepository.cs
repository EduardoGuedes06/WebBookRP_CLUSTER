using System.Data;
using System.Data.Common;
using Dapper;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Repositories;

public class AuthorRepository(IDbConnection connection) : IAuthorRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task<AuthorProfile?> GetProfileAsync(int id)
    {
        await EnsureOpenAsync();
        return await _connection.QueryFirstOrDefaultAsync<AuthorProfile>(
            """
            SELECT Id, Name, Role, AvatarUrl, SecondaryImageUrl, Bio
            FROM author_profiles
            WHERE Id = @Id
            """,
            new { Id = id });
    }

    public async Task<int> UpdateProfileAsync(AuthorProfile profile)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            """
            UPDATE author_profiles SET
                Name = @Name,
                Role = @Role,
                AvatarUrl = @AvatarUrl,
                SecondaryImageUrl = @SecondaryImageUrl,
                Bio = @Bio
            WHERE Id = @Id
            """,
            profile);
    }

    public async Task<IReadOnlyList<AuthorTimelineItem>> GetTimelineAsync(int authorId)
    {
        await EnsureOpenAsync();
        var rows = await _connection.QueryAsync<AuthorTimelineItem>(
            """
            SELECT Id, AuthorId, Year, Title, Description, SortOrder
            FROM author_timeline
            WHERE AuthorId = @AuthorId
            ORDER BY SortOrder ASC, Id ASC
            """,
            new { AuthorId = authorId });
        return rows.ToList();
    }

    public async Task ReplaceTimelineAsync(int authorId, IReadOnlyList<AuthorTimelineItem> items)
    {
        await EnsureOpenAsync();
        var db = (DbConnection)_connection;
        await using var tx = await db.BeginTransactionAsync();

        await _connection.ExecuteAsync(
            "DELETE FROM author_timeline WHERE AuthorId = @AuthorId",
            new { AuthorId = authorId },
            tx);

        foreach (var item in items)
        {
            await _connection.ExecuteAsync(
                """
                INSERT INTO author_timeline (AuthorId, Year, Title, Description, SortOrder)
                VALUES (@AuthorId, @Year, @Title, @Description, @SortOrder)
                """,
                new
                {
                    AuthorId = authorId,
                    item.Year,
                    item.Title,
                    item.Description,
                    item.SortOrder
                },
                tx);
        }

        await tx.CommitAsync();
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();
    }
}
