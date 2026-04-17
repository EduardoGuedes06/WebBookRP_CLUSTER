using System.ComponentModel.DataAnnotations;

namespace WebBookRP_API.DTOs;

public class ServiceResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Unit { get; set; }
    public bool Active { get; set; }
    public bool IsPromotion { get; set; }
    public decimal? PromoPrice { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string? Theme { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ServiceCreateRequestDto
{
    [Required]
    [MaxLength(500)]
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public string? Unit { get; set; }
    public bool Active { get; set; } = true;
    public bool IsPromotion { get; set; }
    public decimal? PromoPrice { get; set; }
    public string? Icon { get; set; }
    public string? Theme { get; set; }
    public string? Description { get; set; }
}

public class ServiceUpdateRequestDto : ServiceCreateRequestDto
{
}
