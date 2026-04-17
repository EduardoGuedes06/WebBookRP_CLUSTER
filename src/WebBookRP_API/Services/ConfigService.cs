using System.Text.Json;
using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;

namespace WebBookRP_API.Services;

public class ConfigService(ISystemSettingsRepository repository) : IConfigService
{
    private const string SystemKey = "SystemConfig";
    private const string HomeKey = "HomeConfig";
    private const string StatsKey = "StatsConfig";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly ISystemSettingsRepository _repository = repository;

    public async Task<SettingResponseDto> GetSystemAsync() => await GetSettingAsync(SystemKey);

    public async Task<SettingResponseDto> UpdateSystemAsync(JsonElement value)
    {
        await _repository.UpsertAsync(SystemKey, JsonSerializer.Serialize(value, JsonOptions));
        return await GetSettingAsync(SystemKey);
    }

    public async Task<SettingResponseDto> GetHomeAsync() => await GetSettingAsync(HomeKey);

    public async Task<SettingResponseDto> UpdateHomeAsync(JsonElement value)
    {
        await _repository.UpsertAsync(HomeKey, JsonSerializer.Serialize(value, JsonOptions));
        return await GetSettingAsync(HomeKey);
    }

    public async Task<StatsResponseDto> GetStatsAsync()
    {
        var json = await _repository.GetValueJsonByKeyAsync(StatsKey);
        if (string.IsNullOrWhiteSpace(json))
            json = "{}";

        using var doc = JsonDocument.Parse(json);
        return new StatsResponseDto { Stats = doc.RootElement.Clone() };
    }

    private async Task<SettingResponseDto> GetSettingAsync(string key)
    {
        var json = await _repository.GetValueJsonByKeyAsync(key);
        if (string.IsNullOrWhiteSpace(json))
            json = "{}";

        using var doc = JsonDocument.Parse(json);
        return new SettingResponseDto
        {
            Key = key,
            Value = doc.RootElement.Clone()
        };
    }
}
