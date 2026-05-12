using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;

namespace WebBookRP_API.Controllers;

[ApiController]
[Route("metrics")]
public class MetricsController(IMetricsService metricsService) : ControllerBase
{
    private readonly IMetricsService _metricsService = metricsService;

    [HttpPost("clicks")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterClick([FromBody] ExternalClickRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        await _metricsService.RegisterClickAsync(request, ip, cancellationToken);
        return Ok();
    }

    [HttpGet("dashboard-summary")]
    //[Authorize]
    [AllowAnonymous]
    public async Task<ActionResult<DashboardSummaryDto>> GetDashboardSummary(CancellationToken cancellationToken)
    {
        return Ok(await _metricsService.GetDashboardSummaryAsync(cancellationToken));
    }

    [HttpGet("funnel")]
    //[Authorize]
    [AllowAnonymous]
    public async Task<ActionResult<LeadFunnelDto>> GetFunnel(CancellationToken cancellationToken)
    {
        return Ok(await _metricsService.GetLeadFunnelAsync(cancellationToken));
    }

    [HttpGet("popular-services")]
    //[Authorize]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<PopularServiceDto>>> GetPopularServices(CancellationToken cancellationToken)
    {
        return Ok(await _metricsService.GetPopularServicesAsync(cancellationToken));
    }

    [HttpGet("traffic-sources")]
    //[Authorize]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<TrafficSourceDto>>> GetTrafficSources(CancellationToken cancellationToken)
    {
        return Ok(await _metricsService.GetTrafficSourcesAsync(cancellationToken));
    }
}
