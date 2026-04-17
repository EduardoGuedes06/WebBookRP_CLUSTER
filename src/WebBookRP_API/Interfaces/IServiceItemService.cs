using WebBookRP_API.DTOs;

namespace WebBookRP_API.Interfaces;

public interface IServiceItemService
{
    Task<IReadOnlyList<ServiceResponseDto>> GetActivePublicAsync();
    Task<IReadOnlyList<ServiceResponseDto>> GetAllForAdminAsync();
    Task<ServiceResponseDto?> GetByIdAsync(Guid id);
    Task<ServiceResponseDto> CreateAsync(ServiceCreateRequestDto request);
    Task<ServiceResponseDto?> UpdateAsync(Guid id, ServiceUpdateRequestDto request);
    Task<bool> DeleteAsync(Guid id);
}
