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
            SELECT Id, PostId, `Text`, UserId, UserName, UserAvatar, AuthorLike, CreatedAt AS CreatedAtUtc
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
            SELECT Id, PostId, `Text`, UserId, UserName, UserAvatar, AuthorLike, CreatedAt AS CreatedAtUtc
            FROM Comments
            WHERE Id = @Id
            """,
            new { Id = commentId });
    }

    public async Task<int> InsertAsync(Comment comment)
    {
        await EnsureOpenAsync();
        const string sql = """
            INSERT INTO Comments (PostId, `Text`, UserId, UserName, UserAvatar, AuthorLike)
            VALUES (@PostId, @Text, @UserId, @UserName, @UserAvatar, @AuthorLike);
            SELECT LAST_INSERT_ID();
            """;
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
    public async Task<Comment?> GetCommentAsync(int postId, int commentId)
    {
        return await _connection.QueryFirstOrDefaultAsync<Comment>(
            "SELECT * FROM Comments WHERE Id = @CommentId AND PostId = @PostId",
            new { CommentId = commentId, PostId = postId });
    }

    public async Task<int> UpdateTextAsync(int postId, int commentId, string text)
    {
        return await _connection.ExecuteAsync(
            "UPDATE Comments SET Text = @Text WHERE Id = @CommentId AND PostId = @PostId",
            new { Text = text, CommentId = commentId, PostId = postId });
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await ((DbConnection)_connection).OpenAsync();
    }
}