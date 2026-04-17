using WebBookRP_API.DTOs;

namespace WebBookRP_API.Interfaces;

public interface IAuthorService
{
    Task<AuthorResponseDto> GetAsync();
    Task<AuthorResponseDto> UpdateAsync(AuthorUpdateRequestDto request);
}
