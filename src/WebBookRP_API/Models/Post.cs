namespace WebBookRP_API.Models;

public class Post
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
    public string Status { get; set; } = "draft";
    public int LikesCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
