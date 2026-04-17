using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBookRP_API.Interfaces;

namespace WebBookRP_API.Controllers;

[ApiController]
[Route("assinatura")]
public class SignatureController(IDatabaseSignatureRepository databaseSignatureRepository) : ControllerBase
{
    private static readonly string[] PublicTables =
    [
        "Books",
        "Services",
        "post_like_ips",
        "Coments",
        "AuthorTimeline",
        "AuthorProfile",
        "Posts"
    ];

    private readonly IDatabaseSignatureRepository _databaseSignatureRepository = databaseSignatureRepository;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<object>> GetPublic(CancellationToken ct)
    {
        var signature = await _databaseSignatureRepository.GetSchemaSignatureForTablesAsync(PublicTables, ct);
        return Ok(new { signature });
    }
}

