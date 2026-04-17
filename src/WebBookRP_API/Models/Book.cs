namespace WebBookRP_API.Models;

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Genre { get; set; }
    public decimal Price { get; set; }
    public decimal? PromoPrice { get; set; }
    public bool Active { get; set; }
    public bool IsPromotion { get; set; }
    public string? Cover { get; set; }
    public string? Synopsis { get; set; }
    public string? CoverSynopsis { get; set; }
    public string? LinkAmazon { get; set; }
    public string? LinkML { get; set; }
    public string? LinkShopee { get; set; }
    public string? LinkGeneric { get; set; }
    public string? FullSynopsis { get; set; }
    public int Pages { get; set; }
    public decimal Rating { get; set; }
    public int Reviews { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
