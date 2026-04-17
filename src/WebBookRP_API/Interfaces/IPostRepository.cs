using WebBookRP_API.Models;

namespace WebBookRP_API.Interfaces;

public interface IPostRepository
{
    Task<IReadOnlyList<Post>> ListAsync(string? status);
    Task<Post?> GetByIdAsync(int id);
    Task<int> InsertAsync(Post post);
    Task<int> UpdateAsync(Post post);
    Task<int> DeleteAsync(int id);
    Task<int> ToggleLikeByIpAsync(int postId, string ipAddress);
}
