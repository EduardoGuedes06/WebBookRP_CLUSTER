using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;

namespace WebBookRP_API.Services;

public class MetricsService(IMetricsRepository metricsRepository) : IMetricsService
{
    private readonly IMetricsRepository _metricsRepository = metricsRepository;

    public async Task RegisterClickAsync(ExternalClickRequestDto request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        await _metricsRepository.RegisterExternalClickAsync(
            request.BookId,
            request.TargetStore.Trim(),
            ipAddress,
            cancellationToken);
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var total = await _metricsRepository.GetTotalExternalClicksAsync(cancellationToken);
        var sources = await _metricsRepository.GetTrafficSourcesAsync(cancellationToken);
        var funnel = await _metricsRepository.GetLeadFunnelAsync(cancellationToken);
        var popular = await _metricsRepository.GetPopularServicesAsync(cancellationToken);

        return new DashboardSummaryDto
        {
            TotalClicks = total,
            TrafficSources = sources.Select(s => new TrafficSourceDto { Store = s.Store, Clicks = s.Clicks }).ToList(),
            LeadFunnel = new LeadFunnelDto
            {
                Pending = funnel.Pending,
                InProduction = funnel.InProduction,
                Completed = funnel.Completed
            },
            PopularServices = popular.Select(p => new PopularServiceDto { ServiceName = p.ServiceName, LeadCount = p.LeadCount }).ToList()
        };
    }

    public async Task<LeadFunnelDto> GetLeadFunnelAsync(CancellationToken cancellationToken = default)
    {
        var f = await _metricsRepository.GetLeadFunnelAsync(cancellationToken);
        return new LeadFunnelDto
        {
            Pending = f.Pending,
            InProduction = f.InProduction,
            Completed = f.Completed
        };
    }

    public async Task<IReadOnlyList<PopularServiceDto>> GetPopularServicesAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _metricsRepository.GetPopularServicesAsync(cancellationToken);
        return rows.Select(p => new PopularServiceDto { ServiceName = p.ServiceName, LeadCount = p.LeadCount }).ToList();
    }

    public async Task<IReadOnlyList<TrafficSourceDto>> GetTrafficSourcesAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _metricsRepository.GetTrafficSourcesAsync(cancellationToken);
        return rows.Select(s => new TrafficSourceDto { Store = s.Store, Clicks = s.Clicks }).ToList();
    }
}
