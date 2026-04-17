namespace WebBookRP_API.Models;

public class AuthorTimelineItem
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public string Year { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}
