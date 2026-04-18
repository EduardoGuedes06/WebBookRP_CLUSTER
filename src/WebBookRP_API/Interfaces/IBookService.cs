using WebBookRP_API.DTOs;

namespace WebBookRP_API.Interfaces;

public interface IBookService
{
    Task<IReadOnlyList<BookResponseDto>> GetAllForAdminAsync();
    Task<IReadOnlyList<BookResponseDto>> GetPublicCatalogAsync();
    Task<BookResponseDto?> GetPublicByIdAsync(Guid id);
    Task<BookResponseDto?> GetByIdForAdminAsync(Guid id);
    Task<BookResponseDto> CreateAsync(BookCreateRequestDto request);
    Task<BookResponseDto?> UpdateAsync(Guid id, BookUpdateRequestDto request);
    Task<bool> DeleteAsync(Guid id);
}
