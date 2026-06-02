using WebBookRP_API.DTOs;

namespace WebBookRP_API.Interfaces;

public interface IPostService
{
    Task<CommentResponseDto?> AddCommentAsync(int postId, CommentCreateRequestDto request, string userId, string userName, string? userAvatar);
    Task<PostDetailResponseDto> CreateAsync(PostCreateRequestDto request);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteCommentAsync(int postId, int commentId);
    Task<bool> DeleteUserCommentAsync(int postId, int commentId, string userId);
    Task<CommentResponseDto?> EditUserCommentAsync(int postId, int commentId, CommentUpdateRequestDto request, string userId);
    Task<PostDetailResponseDto?> GetByIdAsync(int id, string? status, bool isAuthenticated);
    Task<IReadOnlyList<PostDetailResponseDto>> ListAsync(string? status, bool isAuthenticated);
    Task<bool> SetAuthorLikeAsync(int postId, int commentId, bool authorLike);
    Task<int?> ToggleLikeAsync(int postId, string ipAddress);
    Task<PostDetailResponseDto?> UpdateAsync(int id, PostUpdateRequestDto request);
}
