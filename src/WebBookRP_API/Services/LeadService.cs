using WebBookRP_API.DTOs;
using WebBookRP_API.Interfaces;
using WebBookRP_API.Models;

namespace WebBookRP_API.Services;

public class LeadService(ILeadRepository leadRepository, IServiceRepository serviceRepository) : ILeadService
{
    private static readonly HashSet<string> AllowedStatuses =
    [
        "pending", "analyzing", "working", "finished", "cancelled"
    ];

    private readonly ILeadRepository _leadRepository = leadRepository;
    private readonly IServiceRepository _serviceRepository = serviceRepository;

    public async Task<LeadResponseDto> CreatePublicAsync(LeadCreatePublicRequestDto request)
    {
        if (!Guid.TryParse(request.ServiceId, out var serviceId))
            throw new ArgumentException("ServiceId inválido.");

        var service = await _serviceRepository.GetByIdAsync(serviceId);
        if (service is null || !service.Active)
            throw new InvalidOperationException("Serviço não encontrado ou inativo.");

        var now = DateTime.UtcNow;
        var lead = new Lead
        {
            ServiceId = serviceId,
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Description = request.Description,
            Value = 0,
            CreatedAtUtc = now,
            Status = "pending",
            Notes = null
        };

        var id = await _leadRepository.InsertAsync(lead);
        lead.Id = id;
        return Map(lead);
    }

    public async Task<LeadsPagedResponseDto> GetPagedAsync(int page, int pageSize, string? status, string? search)
    {
        if (!string.IsNullOrWhiteSpace(status) && !AllowedStatuses.Contains(status))
            status = null;

        var (items, total) = await _leadRepository.GetPagedAsync(page, pageSize, status, search);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        return new LeadsPagedResponseDto
        {
            Page = Math.Max(1, page),
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = totalPages,
            Items = items.Select(Map).ToList()
        };
    }

    public async Task<LeadResponseDto?> GetByIdAsync(int id)
    {
        var lead = await _leadRepository.GetByIdAsync(id);
        return lead is null ? null : Map(lead);
    }

    public async Task<LeadResponseDto?> UpdateAsync(int id, LeadUpdateRequestDto request)
    {
        var existing = await _leadRepository.GetByIdAsync(id);
        if (existing is null)
            return null;

        if (!Guid.TryParse(request.ServiceId, out var serviceId))
            throw new ArgumentException("ServiceId inválido.");

        var service = await _serviceRepository.GetByIdAsync(serviceId);
        if (service is null)
            throw new InvalidOperationException("Serviço não encontrado.");

        if (!AllowedStatuses.Contains(request.Status))
            throw new ArgumentException("Status inválido.");

        existing.ServiceId = serviceId;
        existing.Name = request.Name.Trim();
        existing.Email = request.Email.Trim();
        existing.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        existing.Description = request.Description;
        existing.Value = request.Value;
        existing.Status = request.Status;
        existing.Notes = request.Notes;

        await _leadRepository.UpdateAsync(existing);
        return Map(existing);
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        if (!AllowedStatuses.Contains(status))
            throw new ArgumentException("Status inválido.");

        var affected = await _leadRepository.UpdateStatusAsync(id, status);
        return affected > 0;
    }

    public async Task<bool> UpdateNotesAsync(int id, string? notes)
    {
        var affected = await _leadRepository.UpdateNotesAsync(id, notes);
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var affected = await _leadRepository.DeleteAsync(id);
        return affected > 0;
    }

    private static LeadResponseDto Map(Lead l) => new()
    {
        Id = l.Id,
        ServiceId = l.ServiceId.ToString(),
        Name = l.Name,
        Email = l.Email,
        Phone = l.Phone,
        Description = l.Description,
        Value = l.Value,
        CreatedAt = DateTime.SpecifyKind(l.CreatedAtUtc, DateTimeKind.Utc),
        Status = l.Status,
        Notes = l.Notes
    };
}
