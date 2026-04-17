using System.ComponentModel.DataAnnotations;

namespace WebBookRP_API.DTOs;

public class LoginRequestDto : IValidatableObject
{
    /// <summary>E-mail cadastrado em Users (preferencial).</summary>
    public string? Email { get; set; }

    /// <summary>Legado: mesmo valor que Email quando o cliente envia "username".</summary>
    public string? Username { get; set; }

    [Required]
    public string Password { get; set; } = string.Empty;

    public string ResolveLoginIdentifier()
    {
        if (!string.IsNullOrWhiteSpace(Email))
            return Email.Trim();
        if (!string.IsNullOrWhiteSpace(Username))
            return Username.Trim();
        return string.Empty;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(Username))
            yield return new ValidationResult("Informe email ou username (e-mail de login).", [nameof(Email), nameof(Username)]);
    }
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

    /// <summary>Identificador do usuário (GUID da tabela Users).</summary>
    public string? UserId { get; set; }

    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }

    /// <summary>Alias para e-mail (compatível com clientes antigos).</summary>
    public string? Username { get; set; }
}
