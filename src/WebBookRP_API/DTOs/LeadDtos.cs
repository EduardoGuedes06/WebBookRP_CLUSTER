using System.ComponentModel.DataAnnotations;

namespace WebBookRP_API.DTOs;

public class LeadCreatePublicRequestDto
{
    [Required]
    public string ServiceId { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Phone { get; set; }

    public string? Description { get; set; }
}

public class LeadResponseDto
{
    public int Id { get; set; }
    public string ServiceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Description { get; set; }
    public decimal Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class LeadUpdateRequestDto
{
    [Required]
    public string ServiceId { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }
    public string? Description { get; set; }
    public decimal Value { get; set; }

    [RegularExpression("^(pending|analyzing|working|finished|cancelled)$")]
    public string Status { get; set; } = "pending";

    public string? Notes { get; set; }
}

public class LeadStatusPatchRequestDto
{
    [Required]
    [RegularExpression("^(pending|analyzing|working|finished|cancelled)$")]
    public string Status { get; set; } = string.Empty;
}

public class LeadNotesPatchRequestDto
{
    public string? Notes { get; set; }
}

public class LeadsPagedResponseDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IReadOnlyList<LeadResponseDto> Items { get; set; } = Array.Empty<LeadResponseDto>();
}
