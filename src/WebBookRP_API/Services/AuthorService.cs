using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Services;

public class AuthorService(IAuthorRepository repository) : IAuthorService
{
    private const int DefaultAuthorId = 1;
    private readonly IAuthorRepository _repository = repository;

    public async Task<AuthorResponseDto> GetAsync()
    {
        var profile = await _repository.GetProfileAsync(DefaultAuthorId)
                      ?? new AuthorProfile { Id = DefaultAuthorId, Name = "Autor" };

        var timeline = await _repository.GetTimelineAsync(DefaultAuthorId);
        return Map(profile, timeline);
    }

    public async Task<AuthorResponseDto> UpdateAsync(AuthorUpdateRequestDto request)
    {
        var profile = await _repository.GetProfileAsync(DefaultAuthorId)
                      ?? new AuthorProfile { Id = DefaultAuthorId, Name = request.Name };

        profile.Name = request.Name;
        profile.Role = request.Role;
        profile.AvatarUrl = request.AvatarUrl;
        profile.SecondaryImageUrl = request.SecondaryImageUrl;
        profile.Bio = request.Bio;
        profile.SocialLinks = request.SocialLinks;

        await _repository.UpdateProfileAsync(profile);

        var items = request.Timeline
            .OrderBy(t => t.SortOrder)
            .Select(t => new AuthorTimelineItem
            {
                Year = t.Year,
                Title = t.Title,
                Description = t.Description,
                SortOrder = t.SortOrder
            })
            .ToList();

        await _repository.ReplaceTimelineAsync(DefaultAuthorId, items);

        var timeline = await _repository.GetTimelineAsync(DefaultAuthorId);
        profile = (await _repository.GetProfileAsync(DefaultAuthorId))!;
        return Map(profile, timeline);
    }

    private static AuthorResponseDto Map(AuthorProfile p, IReadOnlyList<AuthorTimelineItem> timeline) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Role = p.Role,
        AvatarUrl = p.AvatarUrl,
        SecondaryImageUrl = p.SecondaryImageUrl,
        Bio = p.Bio,
        SocialLinks = p.SocialLinks,
        Timeline = timeline
            .Select(t => new AuthorTimelineItemResponseDto
            {
                Id = t.Id,
                Year = t.Year,
                Title = t.Title,
                Description = t.Description,
                SortOrder = t.SortOrder
            })
            .ToList()
    };
}
