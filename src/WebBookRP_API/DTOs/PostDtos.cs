using System.ComponentModel.DataAnnotations;

namespace WebBookRP_API.DTOs;

public class PostListItemResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? CoverType { get; set; }
    public string? CoverColor { get; set; }
    public string? CoverText { get; set; }
    public string? CoverTextColor { get; set; }
    public string? ImageUrl { get; set; }
    public string? Content { get; set; }
    public string? ExternalLink { get; set; }
    public bool AllowLikes { get; set; }
    public bool AllowComments { get; set; }
    public string Status { get; set; } = string.Empty;
    public int LikesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}

public class PostDetailResponseDto : PostListItemResponseDto
{
    public IReadOnlyList<CommentResponseDto> Comments { get; set; } = Array.Empty<CommentResponseDto>();
}

public class PostCreateRequestDto
{
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    public string? Category { get; set; }
    public string? CoverType { get; set; }
    public string? CoverColor { get; set; }
    public string? CoverText { get; set; }
    public string? CoverTextColor { get; set; }
    public string? ImageUrl { get; set; }
    public string? Content { get; set; }
    public string? ExternalLink { get; set; }
    public bool AllowLikes { get; set; } = true;
    public bool AllowComments { get; set; } = true;

    [RegularExpression("^(draft|published)$")]
    public string Status { get; set; } = "draft";
}

public class PostUpdateRequestDto : PostCreateRequestDto
{
}

public class CommentResponseDto
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? UserAvatar { get; set; }
    public bool AuthorLike { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CommentCreateRequestDto
{
    [Required(ErrorMessage = "O texto do comentário é obrigatório.")]
    [MaxLength(1000, ErrorMessage = "O comentário não pode exceder 1000 caracteres.")]
    public string Text { get; set; } = string.Empty;
}

public class AuthorLikePatchRequestDto
{
    [Required]
    public bool AuthorLike { get; set; }
}

public class LikeToggleResponseDto
{
    public int LikesCount { get; set; }
}
