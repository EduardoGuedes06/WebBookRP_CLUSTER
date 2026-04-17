using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;

namespace WebBookRP_API.Controllers;

[ApiController]
[Route("config")]
[Authorize]
public class ConfigController(IConfigService configService) : ControllerBase
{
    private readonly IConfigService _configService = configService;

    [HttpGet("system")]
    public async Task<ActionResult<SettingResponseDto>> GetSystem()
    {
        return Ok(await _configService.GetSystemAsync());
    }

    [HttpPut("system")]
    public async Task<ActionResult<SettingResponseDto>> PutSystem([FromBody] SettingUpdateRequestDto request)
    {
        return Ok(await _configService.UpdateSystemAsync(request.Value));
    }

    [HttpGet("home")]
    public async Task<ActionResult<SettingResponseDto>> GetHome()
    {
        return Ok(await _configService.GetHomeAsync());
    }

    [HttpPut("home")]
    public async Task<ActionResult<SettingResponseDto>> PutHome([FromBody] SettingUpdateRequestDto request)
    {
        return Ok(await _configService.UpdateHomeAsync(request.Value));
    }
}
