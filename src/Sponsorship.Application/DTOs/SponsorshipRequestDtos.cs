using Sponsorship.Domain.Enums;

namespace Sponsorship.Application.DTOs;

public class CreateSponsorshipRequestDto
{
    public string RequestTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int SponsorshipTypeId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? ExpectedBusinessBenefit { get; set; }
    public string? Remarks { get; set; }
    public string? SupportingDocumentUrl { get; set; }
}

public class UpdateSponsorshipRequestDto : CreateSponsorshipRequestDto { }

public class SponsorshipRequestDto
{
    public Guid Id { get; set; }
    public string RequestTitle { get; set; } = string.Empty;
    public Guid RequestorId { get; set; }
    public string RequestorName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int SponsorshipTypeId { get; set; }
    public string SponsorshipTypeName { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? ExpectedBusinessBenefit { get; set; }
    public string? Remarks { get; set; }
    public string? SupportingDocumentUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<WorkflowHistoryDto> WorkflowHistory { get; set; } = new();
}

public class ActionRemarksDto
{
    public string? Remarks { get; set; }
}
