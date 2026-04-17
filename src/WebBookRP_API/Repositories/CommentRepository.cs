using System.Data;
using System.Data.Common;
using Dapper;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Repositories;

public class CommentRepository(IDbConnection connection) : ICommentRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task<IReadOnlyList<Comment>> GetByPostIdAsync(int postId)
    {
        await EnsureOpenAsync();
        var rows = await _connection.QueryAsync<Comment>(
            """
            SELECT Id, PostId, `Text`, GuestName, UserId, AuthorLike, CreatedAt AS CreatedAtUtc
            FROM Comments
            WHERE PostId = @PostId
            ORDER BY CreatedAt ASC
            """,
            new { PostId = postId });
        return rows.ToList();
    }

    public async Task<Comment?> GetByIdAsync(int commentId)
    {
        await EnsureOpenAsync();
        return await _connection.QueryFirstOrDefaultAsync<Comment>(
            """
            SELECT Id, PostId, `Text`, GuestName, UserId, AuthorLike, CreatedAt AS CreatedAtUtc
            FROM Comments
            WHERE Id = @Id
            """,
            new { Id = commentId });
    }

    public async Task<int> InsertAsync(Comment comment)
    {
        await EnsureOpenAsync();
        const string sql = """
            INSERT INTO Comments (PostId, `Text`, GuestName, UserId, AuthorLike)
            VALUES (@PostId, @Text, @GuestName, @UserId, @AuthorLike);
            SELECT LAST_INSERT_ID();
            """;
        // Nota: O CreatedAt é gerado automaticamente pelo banco (DEFAULT CURRENT_TIMESTAMP)
        return await _connection.QuerySingleAsync<int>(sql, comment);
    }

    public async Task<int> DeleteAsync(int postId, int commentId)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            "DELETE FROM Comments WHERE Id = @Id AND PostId = @PostId",
            new { Id = commentId, PostId = postId });
    }

    public async Task<int> UpdateAuthorLikeAsync(int postId, int commentId, bool authorLike)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            """
            UPDATE Comments
            SET AuthorLike = @AuthorLike
            WHERE Id = @Id AND PostId = @PostId
            """,
            new { Id = commentId, PostId = postId, AuthorLike = authorLike });
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await ((DbConnection)_connection).OpenAsync();
    }
}