namespace WebBookRP_API.Models;

public class Lead
{
    public int Id { get; set; }
    public Guid ServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Description { get; set; }
    public decimal Value { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string Status { get; set; } = "pending";
    public string? Notes { get; set; }
}
