using System.ComponentModel.DataAnnotations;

namespace WebBookRP_API.DTOs;

public class DashboardSummaryDto
{
    public int TotalClicks { get; set; }
    public List<TrafficSourceDto> TrafficSources { get; set; } = new();
    public LeadFunnelDto LeadFunnel { get; set; } = new();
    public List<PopularServiceDto> PopularServices { get; set; } = new();
}

public class TrafficSourceDto
{
    public string Store { get; set; } = string.Empty;
    public int Clicks { get; set; }
}

public class LeadFunnelDto
{
    public int Pending { get; set; }
    public int InProduction { get; set; }
    public int Completed { get; set; }
}

public class PopularServiceDto
{
    public string ServiceName { get; set; } = string.Empty;
    public int LeadCount { get; set; }
}

public class ExternalClickRequestDto
{
    public Guid? BookId { get; set; }

    [Required]
    [MaxLength(100)]
    public string TargetStore { get; set; } = string.Empty;
}
