using WebBookRP_API.Models;

namespace WebBookRP_API.Interfaces;

public interface IMetricsRepository
{
    Task RegisterExternalClickAsync(Guid? bookId, string targetStore, string? ipAddress, CancellationToken cancellationToken = default);

    Task<int> GetTotalExternalClicksAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TrafficSourceAggregate>> GetTrafficSourcesAsync(CancellationToken cancellationToken = default);

    Task<LeadFunnelAggregate> GetLeadFunnelAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PopularServiceAggregate>> GetPopularServicesAsync(CancellationToken cancellationToken = default);
}
