using System.Security.Claims;
using WebBookRP_API.DTOs;

namespace WebBookRP_API.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, string? ipAddress, string? userAgent);
    Task<SessionResponseDto> GetSessionAsync(ClaimsPrincipal user);
    Task<LoginResponseDto?> LoginWithGoogleAsync(string credential, string? ipAddress, string? userAgent);
}
