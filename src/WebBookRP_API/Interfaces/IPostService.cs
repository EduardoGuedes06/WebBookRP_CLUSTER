using WebBookRP_API.DTOs;

namespace WebBookRP_API.Interfaces;

public interface IPostService
{
    Task<IReadOnlyList<PostListItemResponseDto>> ListAsync(string? status, bool isAuthenticated);
    Task<PostDetailResponseDto?> GetByIdAsync(int id, string? status, bool isAuthenticated);
    Task<PostDetailResponseDto> CreateAsync(PostCreateRequestDto request);
    Task<PostDetailResponseDto?> UpdateAsync(int id, PostUpdateRequestDto request);
    Task<bool> DeleteAsync(int id);
    Task<int?> ToggleLikeAsync(int postId, string ipAddress);
    Task<CommentResponseDto?> AddCommentAsync(int postId, CommentCreateRequestDto request, string? userId);
    Task<bool> SetAuthorLikeAsync(int postId, int commentId, bool authorLike);
    Task<bool> DeleteCommentAsync(int postId, int commentId);
}
