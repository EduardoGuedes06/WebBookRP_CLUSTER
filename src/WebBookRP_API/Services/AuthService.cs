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

public class AuthService(IOptions<JwtSettings> jwtOptions, ISecurityLogRepository securityLogRepository) : IAuthService
{
    private const string DemoUsername = "admin";
    private const string DemoPassword = "admin";

    private readonly JwtSettings _jwt = jwtOptions.Value;
    private readonly ISecurityLogRepository _securityLogRepository = securityLogRepository;

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, string? ipAddress, string? userAgent)
    {
        var success = string.Equals(request.Username, DemoUsername, StringComparison.Ordinal)
                      && string.Equals(request.Password, DemoPassword, StringComparison.Ordinal);

        await _securityLogRepository.InsertAsync(new SecurityLog
        {
            EmailAttempt = request.Username,
            Success = success,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAtUtc = DateTime.UtcNow
        });

        if (!success)
            return null;

        var token = CreateToken(request.Username);
        return new LoginResponseDto
        {
            Token = token,
            TokenType = "Bearer",
            ExpiresInMinutes = _jwt.ExpirationMinutes
        };
    }

    public Task<SessionResponseDto> GetSessionAsync(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
            return Task.FromResult(new SessionResponseDto { Authenticated = false });

        var name = user.Identity.Name;
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return Task.FromResult(new SessionResponseDto
        {
            Authenticated = true,
            Username = name,
            UserId = id
        });
    }

    private string CreateToken(string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "1"),
            new(JwtRegisteredClaimNames.UniqueName, username),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, "Admin")
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
