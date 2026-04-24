using System.ComponentModel.DataAnnotations;

namespace WebBookRP_API.DTOs;

public class AuthorTimelineItemResponseDto
{
    public int Id { get; set; }
    public string Year { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}

public class AuthorResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string? AvatarUrl { get; set; }
    public string? SecondaryImageUrl { get; set; }
    public string? Bio { get; set; }
    public string? SocialLinks { get; set; }
    public IReadOnlyList<AuthorTimelineItemResponseDto> Timeline { get; set; } = Array.Empty<AuthorTimelineItemResponseDto>();
}

public class AuthorUpdateRequestDto
{
    [Required]
    [MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    public string? Role { get; set; }
    public string? AvatarUrl { get; set; }
    public string? SecondaryImageUrl { get; set; }
    public string? Bio { get; set; }
    public string? SocialLinks { get; set; }

    public List<AuthorTimelineItemUpdateDto> Timeline { get; set; } = new();
}

public class AuthorTimelineItemUpdateDto
{
    public int? Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Year { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public int SortOrder { get; set; }
}
