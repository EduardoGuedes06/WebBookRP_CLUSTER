namespace WebBookRP_API.Models;

/// <summary>
/// Resultados agregados vindos do repositório de métricas (mapeados para DTOs na service).
/// </summary>
public record TrafficSourceAggregate(string Store, int Clicks);

public record PopularServiceAggregate(string ServiceName, int LeadCount);

public record LeadFunnelAggregate(int Pending, int InProduction, int Completed);
