using WebBookRP_API.DTOs;

namespace WebBookRP_API.Interfaces;

public interface ILeadService
{
    Task<LeadResponseDto> CreatePublicAsync(LeadCreatePublicRequestDto request);
    Task<LeadsPagedResponseDto> GetPagedAsync(int page, int pageSize, string? status, string? search);
    Task<LeadResponseDto?> GetByIdAsync(int id);
    Task<LeadResponseDto?> UpdateAsync(int id, LeadUpdateRequestDto request);
    Task<bool> UpdateStatusAsync(int id, string status);
    Task<bool> UpdateNotesAsync(int id, string? notes);
    Task<bool> DeleteAsync(int id);
}
