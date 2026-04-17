namespace WebBookRP_API.Interfaces;

public interface IDatabaseSignatureRepository
{
    Task<string> GetCurrentDatabaseSignatureAsync(CancellationToken ct = default);
    Task<string> GetSchemaSignatureForTablesAsync(IReadOnlyCollection<string> tableNames, CancellationToken ct = default);
}

