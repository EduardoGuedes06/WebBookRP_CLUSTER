namespace WebBookRP_API.Models;

public class Comment
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? GuestName { get; set; }
    public string? UserId { get; set; }
    public bool AuthorLike { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
