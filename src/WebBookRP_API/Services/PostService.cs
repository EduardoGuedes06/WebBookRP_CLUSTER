using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Services;

public class PostService(IPostRepository postRepository, ICommentRepository commentRepository) : IPostService
{
    private const int MaxImageUrlChars = 4 * 1024 * 1024;

    private readonly IPostRepository _postRepository = postRepository;
    private readonly ICommentRepository _commentRepository = commentRepository;

    public async Task<IReadOnlyList<PostDetailResponseDto>> ListAsync(string? status, bool isAuthenticated)
    {
        var effective = !isAuthenticated ? "published" : status;
        var posts = await _postRepository.ListAsync(effective);

        var result = new List<PostDetailResponseDto>();

        foreach (var post in posts)
        {
            var comments = await _commentRepository.GetByPostIdAsync(post.Id);
            result.Add(MapDetail(post, comments.Select(MapComment).ToList()));
        }
            
        return result;
    }

    public async Task<PostDetailResponseDto?> GetByIdAsync(int id, string? status, bool isAuthenticated)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post is null)
            return null;

        if (!isAuthenticated && post.Status != "published")
            return null;

        if (isAuthenticated && !string.IsNullOrWhiteSpace(status) && post.Status != status)
            return null;

        var comments = await _commentRepository.GetByPostIdAsync(id);
        return MapDetail(post, comments.Select(MapComment).ToList());
    }

    public async Task<PostDetailResponseDto> CreateAsync(PostCreateRequestDto request)
    {
        ValidateImageUrl(request.ImageUrl);

        var now = DateTime.UtcNow;
        var post = new Post
        {
            Title = request.Title,
            Category = request.Category,
            CoverType = request.CoverType,
            CoverColor = request.CoverColor,
            CoverText = request.CoverText,
            CoverTextColor = request.CoverTextColor,
            ImageUrl = request.ImageUrl,
            Content = request.Content,
            ExternalLink = request.ExternalLink,
            AllowLikes = request.AllowLikes,
            AllowComments = request.AllowComments,
            Status = request.Status,
            LikesCount = 0,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        var id = await _postRepository.InsertAsync(post);
        post.Id = id;

        return MapDetail(post, []);
    }

    public async Task<PostDetailResponseDto?> UpdateAsync(int id, PostUpdateRequestDto request)
    {
        var existing = await _postRepository.GetByIdAsync(id);
        if (existing is null)
            return null;

        var now = DateTime.UtcNow;
        existing.Title = request.Title;
        existing.Category = request.Category;
        existing.CoverType = request.CoverType;
        existing.CoverColor = request.CoverColor;
        existing.CoverText = request.CoverText;
        existing.CoverTextColor = request.CoverTextColor;
        ValidateImageUrl(request.ImageUrl);
        existing.ImageUrl = request.ImageUrl;
        existing.Content = request.Content;
        existing.ExternalLink = request.ExternalLink;
        existing.AllowLikes = request.AllowLikes;
        existing.AllowComments = request.AllowComments;
        existing.Status = request.Status;
        existing.UpdatedAtUtc = now;

        await _postRepository.UpdateAsync(existing);

        var comments = await _commentRepository.GetByPostIdAsync(id);
        return MapDetail(existing, comments.Select(MapComment).ToList());
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var affected = await _postRepository.DeleteAsync(id);
        return affected > 0;
    }

    public async Task<int?> ToggleLikeAsync(int postId, string ipAddress)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post is null || !post.AllowLikes || post.Status != "published")
            return null;

        return await _postRepository.ToggleLikeByIpAsync(postId, ipAddress);
    }

    public async Task<CommentResponseDto?> AddCommentAsync(int postId, CommentCreateRequestDto request, string userId, string userName, string? userAvatar)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post is null || !post.AllowComments)
            return null;

        if (post.Status != "published")
            return null;

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Usuário não autenticado.");

        var now = DateTime.UtcNow;
        var comment = new Comment
        {
            PostId = postId,
            Text = request.Text.Trim(),
            UserId = userId,
            UserName = userName,
            UserAvatar = userAvatar,
            AuthorLike = false,
            CreatedAtUtc = now
        };

        var id = await _commentRepository.InsertAsync(comment);
        comment.Id = id;
        return MapComment(comment);
    }

    public async Task<bool> SetAuthorLikeAsync(int postId, int commentId, bool authorLike)
    {
        var affected = await _commentRepository.UpdateAuthorLikeAsync(postId, commentId, authorLike);
        return affected > 0;
    }

    public async Task<bool> DeleteCommentAsync(int postId, int commentId)
    {
        var affected = await _commentRepository.DeleteAsync(postId, commentId);
        return affected > 0;
    }

    private static void ValidateImageUrl(string? imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return;

        if (imageUrl.Length > MaxImageUrlChars)
            throw new ArgumentException($"ImageUrl excede o limite de {MaxImageUrlChars / (1024 * 1024)} MB.");
    }

    private static PostListItemResponseDto MapListItem(Post p) => new()
    {
        Id = p.Id,
        Title = p.Title,
        Category = p.Category,
        CoverType = p.CoverType,
        CoverColor = p.CoverColor,
        CoverText = p.CoverText,
        CoverTextColor = p.CoverTextColor,
        ImageUrl = p.ImageUrl,
        Content = p.Content,
        ExternalLink = p.ExternalLink,
        AllowLikes = p.AllowLikes,
        AllowComments = p.AllowComments,
        Status = p.Status,
        LikesCount = p.LikesCount,
        CreatedAt = DateTime.SpecifyKind(p.CreatedAtUtc, DateTimeKind.Utc),
        UpdatedAt = DateTime.SpecifyKind(p.UpdatedAtUtc, DateTimeKind.Utc)
    };

    private static PostDetailResponseDto MapDetail(Post p, IReadOnlyList<CommentResponseDto> comments)
    {
        var list = MapListItem(p);
        return new PostDetailResponseDto
        {
            Id = list.Id,
            Title = list.Title,
            Category = list.Category,
            CoverType = list.CoverType,
            CoverColor = list.CoverColor,
            CoverText = list.CoverText,
            CoverTextColor = list.CoverTextColor,
            ImageUrl = list.ImageUrl,
            Content = list.Content,
            ExternalLink = list.ExternalLink,
            AllowLikes = list.AllowLikes,
            AllowComments = list.AllowComments,
            Status = list.Status,
            LikesCount = list.LikesCount,
            CreatedAt = list.CreatedAt,
            UpdatedAt = list.UpdatedAt,
            Comments = comments
        };
    }

    public async Task<bool> DeleteUserCommentAsync(int postId, int commentId, string userId)
    {
        var comment = await _commentRepository.GetCommentAsync(postId, commentId);
        if (comment is null) return false;

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("Você não tem permissão para deletar este comentário.");

        var affected = await _commentRepository.DeleteAsync(postId, commentId);
        return affected > 0;
    }

    public async Task<CommentResponseDto?> EditUserCommentAsync(int postId, int commentId, CommentUpdateRequestDto request, string userId)
    {
        var comment = await _commentRepository.GetCommentAsync(postId, commentId);
        if (comment is null) return null;

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("Você não tem permissão para editar este comentário.");

        comment.Text = request.Text.Trim();
        var affected = await _commentRepository.UpdateTextAsync(postId, commentId, comment.Text);

        return affected > 0 ? MapComment(comment) : null;
    }

    private static CommentResponseDto MapComment(Comment c) => new()
    {
        Id = c.Id,
        PostId = c.PostId,
        Text = c.Text,
        UserId = c.UserId,
        UserName = c.UserName,
        UserAvatar = c.UserAvatar,
        AuthorLike = c.AuthorLike,
        CreatedAt = DateTime.SpecifyKind(c.CreatedAtUtc, DateTimeKind.Utc)
    };


}