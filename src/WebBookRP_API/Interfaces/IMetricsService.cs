using WebBookRP_API.DTOs;

namespace WebBookRP_API.Interfaces;

public interface IMetricsService
{
    Task RegisterClickAsync(ExternalClickRequestDto request, string? ipAddress, CancellationToken cancellationToken = default);

    Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);

    Task<LeadFunnelDto> GetLeadFunnelAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PopularServiceDto>> GetPopularServicesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TrafficSourceDto>> GetTrafficSourcesAsync(CancellationToken cancellationToken = default);
}
