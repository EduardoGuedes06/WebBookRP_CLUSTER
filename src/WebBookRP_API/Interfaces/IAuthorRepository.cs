using WebBookRP_API.Models;

namespace WebBookRP_API.Interfaces;

public interface IAuthorRepository
{
    Task<AuthorProfile?> GetProfileAsync(int id);
    Task<int> UpdateProfileAsync(AuthorProfile profile);
    Task<IReadOnlyList<AuthorTimelineItem>> GetTimelineAsync(int authorId);
    Task ReplaceTimelineAsync(int authorId, IReadOnlyList<AuthorTimelineItem> items);
}
