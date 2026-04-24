namespace WebBookRP_API.Models;

public class AuthorProfile
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Role { get; set; }
    public string? AvatarUrl { get; set; }
    public string? SecondaryImageUrl { get; set; }
    public string? Bio { get; set; }
    public string? SocialLinks { get; set; }
}
