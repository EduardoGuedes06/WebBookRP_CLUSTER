using WebBookRP_API.Models;

namespace WebBookRP_API.Interfaces;

public interface IServiceRepository
{
    Task<IReadOnlyList<ServiceItem>> GetActiveAsync();
    Task<IReadOnlyList<ServiceItem>> GetAllAsync();
    Task<ServiceItem?> GetByIdAsync(Guid id);
    Task<int> InsertAsync(ServiceItem item);
    Task<int> UpdateAsync(ServiceItem item);
    Task<int> DeleteAsync(Guid id);
}
