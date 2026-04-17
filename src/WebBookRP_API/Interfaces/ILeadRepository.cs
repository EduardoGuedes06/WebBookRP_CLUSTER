using WebBookRP_API.Models;

namespace WebBookRP_API.Interfaces;

public interface ILeadRepository
{
    Task<int> InsertAsync(Lead lead);
    Task<Lead?> GetByIdAsync(int id);
    Task<(IReadOnlyList<Lead> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? status, string? search);
    Task<int> UpdateAsync(Lead lead);
    Task<int> UpdateStatusAsync(int id, string status);
    Task<int> UpdateNotesAsync(int id, string? notes);
    Task<int> DeleteAsync(int id);
}
