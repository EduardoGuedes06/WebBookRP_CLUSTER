namespace WebBookRP_API.Models;

public class SecurityLog
{
    public int Id { get; set; }
    public string EmailAttempt { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
