using WebBookRP_API.DTOs;
using WebBookRP_API.Infrastructure;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Services;

public class ServiceItemService(IServiceRepository repository) : IServiceItemService
{
    private readonly IServiceRepository _repository = repository;

    public async Task<IReadOnlyList<ServiceResponseDto>> GetActivePublicAsync()
    {
        var items = await _repository.GetActiveAsync();
        return items.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<ServiceResponseDto>> GetAllForAdminAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(Map).ToList();
    }

    public async Task<ServiceResponseDto?> GetByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item is null ? null : Map(item);
    }

    public async Task<ServiceResponseDto> CreateAsync(ServiceCreateRequestDto request)
    {
        var now = DateTime.UtcNow;
        var entity = new ServiceItem
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            Unit = request.Unit,
            Active = request.Active,
            IsPromotion = request.IsPromotion,
            PromoPrice = request.PromoPrice,
            Icon = NormalizeIcon(request.Icon),
            Theme = request.Theme,
            Description = request.Description,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        await _repository.InsertAsync(entity);
        return Map(entity);
    }

    public async Task<ServiceResponseDto?> UpdateAsync(Guid id, ServiceUpdateRequestDto request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            return null;

        var now = DateTime.UtcNow;
        existing.Name = request.Name;
        existing.Price = request.Price;
        existing.Unit = request.Unit;
        existing.Active = request.Active;
        existing.IsPromotion = request.IsPromotion;
        existing.PromoPrice = request.PromoPrice;
        existing.Icon = NormalizeIcon(request.Icon);
        existing.Theme = request.Theme;
        existing.Description = request.Description;
        existing.UpdatedAtUtc = now;

        await _repository.UpdateAsync(existing);
        return Map(existing);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var affected = await _repository.DeleteAsync(id);
        return affected > 0;
    }

    private static string? NormalizeIcon(string? icon) =>
        string.IsNullOrWhiteSpace(icon) ? MediaPlaceholders.ServiceIcon : icon.Trim();

    private static ServiceResponseDto Map(ServiceItem s) => new()
    {
        Id = s.Id.ToString(),
        Name = s.Name,
        Price = s.Price,
        Unit = s.Unit,
        Active = s.Active,
        IsPromotion = s.IsPromotion,
        PromoPrice = s.PromoPrice,
        Icon = string.IsNullOrWhiteSpace(s.Icon) ? MediaPlaceholders.ServiceIcon : s.Icon,
        Theme = s.Theme,
        Description = s.Description,
        CreatedAt = DateTime.SpecifyKind(s.CreatedAtUtc, DateTimeKind.Utc),
        UpdatedAt = DateTime.SpecifyKind(s.UpdatedAtUtc, DateTimeKind.Utc)
    };
}
