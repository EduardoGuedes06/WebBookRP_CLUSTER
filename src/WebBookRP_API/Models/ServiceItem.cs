namespace WebBookRP_API.Models;

public class ServiceItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Unit { get; set; }
    public bool Active { get; set; }
    public bool IsPromotion { get; set; }
    public decimal? PromoPrice { get; set; }
    public string? Icon { get; set; }
    public string? Theme { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
