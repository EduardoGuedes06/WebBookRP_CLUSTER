using System.Text.Json;
using WebBookRP_API.DTOs;

namespace WebBookRP_API.Interfaces;

public interface IConfigService
{
    Task<SettingResponseDto> GetSystemAsync();
    Task<SettingResponseDto> UpdateSystemAsync(JsonElement value);
    Task<SettingResponseDto> GetHomeAsync();
    Task<SettingResponseDto> UpdateHomeAsync(JsonElement value);
    Task<StatsResponseDto> GetStatsAsync();
}
