using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;

namespace WebBookRP_API.Controllers;

[ApiController]
[Route("stats")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class StatsController(IConfigService configService) : ControllerBase
{
    private readonly IConfigService _configService = configService;

    [HttpGet]
    public async Task<ActionResult<StatsResponseDto>> Get()
    {
        return Ok(await _configService.GetStatsAsync());
    }
}
