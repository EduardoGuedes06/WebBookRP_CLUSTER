using System.Data;
using System.Data.Common;
using Dapper;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Repositories;

public class PostRepository(IDbConnection connection) : IPostRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task<IReadOnlyList<Post>> ListAsync(string? status)
    {
        await EnsureOpenAsync();
        var rows = await _connection.QueryAsync<Post>(
            """
            SELECT Id, Title, Category, CoverType, CoverColor, CoverText, CoverTextColor,
                   ImageUrl, Content,
                   CAST(NULL AS CHAR) AS ExternalLink,
                   1 AS AllowLikes,
                   1 AS AllowComments,
                   Status, LikesCount,
                   CreatedAt AS CreatedAtUtc,
                   CreatedAt AS UpdatedAtUtc
            FROM Posts
            WHERE (@Status IS NULL OR Status = @Status)
            ORDER BY CreatedAt DESC
            """,
            new { Status = status });
        return rows.ToList();
    }

    public async Task<Post?> GetByIdAsync(int id)
    {
        await EnsureOpenAsync();
        return await _connection.QueryFirstOrDefaultAsync<Post>(
            """
            SELECT Id, Title, Category, CoverType, CoverColor, CoverText, CoverTextColor,
                   ImageUrl, Content,
                   CAST(NULL AS CHAR) AS ExternalLink,
                   1 AS AllowLikes,
                   1 AS AllowComments,
                   Status, LikesCount,
                   CreatedAt AS CreatedAtUtc,
                   CreatedAt AS UpdatedAtUtc
            FROM Posts
            WHERE Id = @Id
            """,
            new { Id = id });
    }

    public async Task<int> InsertAsync(Post post)
    {
        await EnsureOpenAsync();
        const string sql = """
            INSERT INTO Posts (
                Title, Category, CoverType, CoverColor, CoverText, CoverTextColor,
                ImageUrl, Content, Status, LikesCount
            ) VALUES (
                @Title, @Category, @CoverType, @CoverColor, @CoverText, @CoverTextColor,
                @ImageUrl, @Content, @Status, @LikesCount
            );
            SELECT LAST_INSERT_ID();
            """;
        return await _connection.QuerySingleAsync<int>(sql, post);
    }

    public async Task<int> UpdateAsync(Post post)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(
            """
            UPDATE Posts SET
                Title = @Title,
                Category = @Category,
                CoverType = @CoverType,
                CoverColor = @CoverColor,
                CoverText = @CoverText,
                CoverTextColor = @CoverTextColor,
                ImageUrl = @ImageUrl,
                Content = @Content,
                Status = @Status,
                LikesCount = @LikesCount
            WHERE Id = @Id
            """,
            post);
    }

    public async Task<int> DeleteAsync(int id)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync("DELETE FROM Posts WHERE Id = @Id", new { Id = id });
    }

    public async Task<int> ToggleLikeByIpAsync(int postId, string ipAddress)
    {
        await EnsureOpenAsync();
        var db = (DbConnection)_connection;
        await using var tx = await db.BeginTransactionAsync();

        // Nota: A tabela post_like_ips não existe no seu create.sql manual.
        // Se desejar manter essa funcionalidade, adicione-a ao banco.
        var deleted = await _connection.ExecuteAsync(
            "DELETE FROM post_like_ips WHERE PostId = @PostId AND IpAddress = @IpAddress",
            new { PostId = postId, IpAddress = ipAddress },
            tx);

        var now = DateTime.UtcNow;

        if (deleted > 0)
        {
            await _connection.ExecuteAsync(
                """
                UPDATE Posts
                SET LikesCount = GREATEST(LikesCount - 1, 0)
                WHERE Id = @PostId
                """,
                new { PostId = postId },
                tx);
        }
        else
        {
            await _connection.ExecuteAsync(
                """
                INSERT INTO post_like_ips (PostId, IpAddress, CreatedAtUtc)
                VALUES (@PostId, @IpAddress, @Now)
                """,
                new { PostId = postId, IpAddress = ipAddress, Now = now },
                tx);

            await _connection.ExecuteAsync(
                """
                UPDATE Posts
                SET LikesCount = LikesCount + 1
                WHERE Id = @PostId
                """,
                new { PostId = postId },
                tx);
        }

        var likes = await _connection.QuerySingleAsync<int>(
            "SELECT LikesCount FROM Posts WHERE Id = @PostId",
            new { PostId = postId },
            tx);

        await tx.CommitAsync();
        return likes;
    }

    private async Task EnsureOpenAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await ((DbConnection)_connection).OpenAsync();
    }
}