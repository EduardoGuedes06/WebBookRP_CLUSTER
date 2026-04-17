using WebBookRP_API.DTOs;
using WebBookRP_API.Infrastructure;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Services;

public class BookService(IBookRepository repository) : IBookService
{
    private readonly IBookRepository _repository = repository;

    public async Task<IReadOnlyList<BookResponseDto>> GetAllForAdminAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<BookResponseDto>> GetPublicCatalogAsync()
    {
        var items = await _repository.GetActiveCatalogAsync();
        return items.Select(Map).ToList();
    }

    public async Task<BookResponseDto?> GetPublicByIdAsync(Guid id)
    {
        var book = await _repository.GetByIdAsync(id);
        if (book is null || !book.Active)
            return null;
        return Map(book);
    }

    public async Task<BookResponseDto> CreateAsync(BookCreateRequestDto request)
    {
        var now = DateTime.UtcNow;
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Genre = request.Genre,
            Price = request.Price,
            PromoPrice = request.PromoPrice,
            Active = request.Active,
            IsPromotion = request.IsPromotion,
            Cover = NormalizeCover(request.Cover),
            Synopsis = request.Synopsis,
            CoverSynopsis = request.CoverSynopsis,
            LinkAmazon = request.LinkAmazon,
            LinkML = request.LinkML,
            LinkShopee = request.LinkShopee,
            LinkGeneric = request.LinkGeneric,
            FullSynopsis = request.FullSynopsis,
            Pages = request.Pages,
            Rating = request.Rating,
            Reviews = request.Reviews,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        await _repository.InsertAsync(book);
        return Map(book);
    }

    public async Task<BookResponseDto?> UpdateAsync(Guid id, BookUpdateRequestDto request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            return null;

        var now = DateTime.UtcNow;
        existing.Title = request.Title;
        existing.Genre = request.Genre;
        existing.Price = request.Price;
        existing.PromoPrice = request.PromoPrice;
        existing.Active = request.Active;
        existing.IsPromotion = request.IsPromotion;
        existing.Cover = NormalizeCover(request.Cover);
        existing.Synopsis = request.Synopsis;
        existing.CoverSynopsis = request.CoverSynopsis;
        existing.LinkAmazon = request.LinkAmazon;
        existing.LinkML = request.LinkML;
        existing.LinkShopee = request.LinkShopee;
        existing.LinkGeneric = request.LinkGeneric;
        existing.FullSynopsis = request.FullSynopsis;
        existing.Pages = request.Pages;
        existing.Rating = request.Rating;
        existing.Reviews = request.Reviews;
        existing.UpdatedAtUtc = now;

        await _repository.UpdateAsync(existing);
        return Map(existing);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var affected = await _repository.DeleteAsync(id);
        return affected > 0;
    }

    private static string? NormalizeCover(string? cover) =>
        string.IsNullOrWhiteSpace(cover) ? MediaPlaceholders.BookCover : cover.Trim();

    private static BookResponseDto Map(Book b) => new()
    {
        Id = b.Id.ToString(),
        Title = b.Title,
        Genre = b.Genre,
        Price = b.Price,
        PromoPrice = b.PromoPrice,
        Active = b.Active,
        IsPromotion = b.IsPromotion,
        Cover = string.IsNullOrWhiteSpace(b.Cover) ? MediaPlaceholders.BookCover : b.Cover,
        Synopsis = b.Synopsis,
        CoverSynopsis = b.CoverSynopsis,
        LinkAmazon = b.LinkAmazon,
        LinkML = b.LinkML,
        LinkShopee = b.LinkShopee,
        LinkGeneric = b.LinkGeneric,
        FullSynopsis = b.FullSynopsis,
        Pages = b.Pages,
        Rating = b.Rating,
        Reviews = b.Reviews,
        CreatedAt = DateTime.SpecifyKind(b.CreatedAtUtc, DateTimeKind.Utc),
        UpdatedAt = DateTime.SpecifyKind(b.UpdatedAtUtc, DateTimeKind.Utc)
    };
}
