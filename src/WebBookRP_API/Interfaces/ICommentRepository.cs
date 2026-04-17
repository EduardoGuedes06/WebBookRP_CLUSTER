using WebBookRP_API.Models;

namespace WebBookRP_API.Interfaces;

public interface ICommentRepository
{
    Task<IReadOnlyList<Comment>> GetByPostIdAsync(int postId);
    Task<Comment?> GetByIdAsync(int commentId);
    Task<int> InsertAsync(Comment comment);
    Task<int> DeleteAsync(int postId, int commentId);
    Task<int> UpdateAuthorLikeAsync(int postId, int commentId, bool authorLike);
}
