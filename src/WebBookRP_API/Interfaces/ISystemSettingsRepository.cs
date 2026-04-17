namespace WebBookRP_API.Interfaces;

public interface ISystemSettingsRepository
{
    Task<string?> GetValueJsonByKeyAsync(string key);
    Task UpsertAsync(string key, string valueJson);
}
