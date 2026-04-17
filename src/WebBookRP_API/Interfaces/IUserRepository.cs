using WebBookRP_API.Models;

namespace WebBookRP_API.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
}
