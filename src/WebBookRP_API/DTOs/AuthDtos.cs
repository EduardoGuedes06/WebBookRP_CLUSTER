using System.ComponentModel.DataAnnotations;

namespace WebBookRP_API.DTOs;

public class LoginRequestDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresInMinutes { get; set; }
}

public class SessionResponseDto
{
    public bool Authenticated { get; set; }
    public string? Username { get; set; }
    public string? UserId { get; set; }
}
