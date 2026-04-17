using WebBookRP_API.Models;

namespace WebBookRP_API.Interfaces;

public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllAsync();
    Task<IReadOnlyList<Book>> GetActiveCatalogAsync();
    Task<Book?> GetByIdAsync(Guid id);
    Task<int> InsertAsync(Book book);
    Task<int> UpdateAsync(Book book);
    Task<int> DeleteAsync(Guid id);
}
