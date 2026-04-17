using WebBookRP_API.Models;

namespace WebBookRP_API.Interfaces;

public interface ISecurityLogRepository
{
    Task<int> InsertAsync(SecurityLog log);
}
