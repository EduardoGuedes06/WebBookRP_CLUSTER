namespace WebBookRP_API.Models;

public class Comment
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? UserAvatar { get; set; }
    public bool AuthorLike { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
