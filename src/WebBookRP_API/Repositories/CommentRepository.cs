using System.Data;
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
            SELECT Id, PostId, `Text` AS Text, GuestName, UserId, AuthorLike, CreatedAtUtc
            FROM comments
            WHERE PostId = @PostId
            ORDER BY CreatedAtUtc ASC
            """,
            new { PostId = postId });
        return rows.ToList();
    }

    public async Task<Comment?> GetByIdAsync(int commentId)
    {
        await EnsureOpenAsync();
        return await _connection.QueryFirstOrDefaultAsync<Comment>(
            """
            SELECT Id, PostId, `Text` AS Text, GuestName, UserId, AuthorLike, CreatedAtUtc
            FROM comments
            WHERE Id = @Id
            """,
            new { Id = commentId });
    }

    public async Task<int> InsertAsync(Comment comment)
    {
        await EnsureOpenAsync();
        const string sql = """
            INSERT INTO comments (PostId, `Text`, GuestName, UserId, AuthorLike, CreatedAtUtc)
            VALUES (@PostId, @Text, @GuestName, @UserId, @AuthorLike, @CreatedAtUtc);
            SELECT LAST_INSERT_ID();
            """;
        return await _connection.QuerySingleAsync<int>(sql, comment);
    }

    public async Task<int> DeleteAsync(int postId, int commentId)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            "DELETE FROM comments WHERE Id = @Id AND PostId = @PostId",
            new { Id = commentId, PostId = postId });
    }

    public async Task<int> UpdateAuthorLikeAsync(int postId, int commentId, bool authorLike)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            """
            UPDATE comments
            SET AuthorLike = @AuthorLike
            WHERE Id = @Id AND PostId = @PostId
            """,
            new { Id = commentId, PostId = postId, AuthorLike = authorLike });
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();
    }
}
