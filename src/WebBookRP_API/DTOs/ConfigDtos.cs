using System.Text.Json;

namespace WebBookRP_API.DTOs;

public class SettingResponseDto
{
    public string Key { get; set; } = string.Empty;
    public JsonElement Value { get; set; }
}

public class SettingUpdateRequestDto
{
    public JsonElement Value { get; set; }
}

public class StatsResponseDto
{
    public JsonElement Stats { get; set; }
}
