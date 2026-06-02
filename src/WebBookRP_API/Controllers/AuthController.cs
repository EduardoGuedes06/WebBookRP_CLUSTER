using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;

namespace WebBookRP_API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService, IDatabaseSignatureRepository databaseSignatureRepository) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IDatabaseSignatureRepository _databaseSignatureRepository = databaseSignatureRepository;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        var result = await _authService.LoginAsync(request, ip, ua);
        if (result is null)
            return Unauthorized();

        return Ok(result);
    }

    [HttpPost("google")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> GoogleLogin([FromBody] GoogleLoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();

        var result = await _authService.LoginWithGoogleAsync(request.Credential, ip, ua);
        if (result is null)
            return Unauthorized(new { message = "Falha na autenticação com o Google." });

        return Ok(result);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public IActionResult Logout()
    {
        return NoContent();
    }

    [HttpGet("session")]
    [AllowAnonymous]
    public async Task<ActionResult<SessionResponseDto>> Session()
    {
        return Ok(await _authService.GetSessionAsync(User));
    }

    [HttpGet("assinatura")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<ActionResult<object>> Assinatura(CancellationToken ct)
    {
        var signature = await _databaseSignatureRepository.GetCurrentDatabaseSignatureAsync(ct);
        return Ok(new { signature });
    }
}
