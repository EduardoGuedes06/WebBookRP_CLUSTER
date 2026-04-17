using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebBookRP_API.Configuration;
using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Services;

public class AuthService(
    IOptions<JwtSettings> jwtOptions,
    IUserRepository userRepository,
    ISecurityLogRepository securityLogRepository) : IAuthService
{
    private readonly JwtSettings _jwt = jwtOptions.Value;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ISecurityLogRepository _securityLogRepository = securityLogRepository;

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, string? ipAddress, string? userAgent)
    {
        var login = request.ResolveLoginIdentifier();
        if (string.IsNullOrWhiteSpace(login))
        {
            await LogAttemptAsync(string.Empty, false, ipAddress, userAgent);
            return null;
        }

        var user = await _userRepository.GetByEmailAsync(login);
        var success = user is not null && VerifyPassword(request.Password, user.PasswordHash);

        await LogAttemptAsync(login, success, ipAddress, userAgent);

        if (!success || user is null)
            return null;

        var token = CreateToken(user);
        return new LoginResponseDto
        {
            Token = token,
            TokenType = "Bearer",
            ExpiresInMinutes = _jwt.ExpirationMinutes
        };
    }

    public Task<SessionResponseDto> GetSessionAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
            return Task.FromResult(new SessionResponseDto { Authenticated = false });

        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var name = principal.FindFirstValue(ClaimTypes.Name);
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var role = principal.FindFirstValue(ClaimTypes.Role);

        return Task.FromResult(new SessionResponseDto
        {
            Authenticated = true,
            UserId = id,
            Name = name,
            Email = email,
            Role = role,
            Username = email ?? name
        });
    }

    private async Task LogAttemptAsync(string emailAttempt, bool success, string? ipAddress, string? userAgent)
    {
        await _securityLogRepository.InsertAsync(new SecurityLog
        {
            EmailAttempt = emailAttempt,
            Success = success,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAtUtc = DateTime.UtcNow
        });
    }

    private static bool VerifyPassword(string plain, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(plain) || string.IsNullOrWhiteSpace(storedHash))
            return false;

        storedHash = storedHash.Trim();
        if (storedHash.StartsWith("$2", StringComparison.Ordinal))
            return BCrypt.Net.BCrypt.Verify(plain, storedHash);

        // Fallback inseguro apenas para migração legada (evite em produção)
        return string.Equals(plain, storedHash, StringComparison.Ordinal);
    }

    private string CreateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, string.IsNullOrWhiteSpace(user.Role) ? "Admin" : user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
