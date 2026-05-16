namespace Sponsorship.BlazorUI.Models;

public class SponsorshipTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class CreateSponsorshipTypeDto
{
    public string Name { get; set; } = "";
}

public class SponsorshipRequestDto
{
    public string Id { get; set; } = "";
    public string RequestTitle { get; set; } = "";
    public string RequestorName { get; set; } = "";
    public string Department { get; set; } = "";
    public string SponsorshipTypeName { get; set; } = "";
    public int SponsorshipTypeId { get; set; }
    public string EventName { get; set; } = "";
    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Purpose { get; set; } = "";
    public string? ExpectedBusinessBenefit { get; set; }
    public string? Remarks { get; set; }
    public string? SupportingDocumentUrl { get; set; }
    public string Status { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<WorkflowHistoryDto> WorkflowHistory { get; set; } = new();
}

public class CreateSponsorshipRequestDto
{
    public string RequestTitle { get; set; } = "";
    public string Department { get; set; } = "";
    public int SponsorshipTypeId { get; set; }
    public string EventName { get; set; } = "";
    public DateTime EventDate { get; set; } = DateTime.Today.AddMonths(1);
    public decimal RequestedAmount { get; set; }
    public string Purpose { get; set; } = "";
    public string? ExpectedBusinessBenefit { get; set; }
    public string? SupportingDocumentUrl { get; set; }
}

public class UpdateSponsorshipRequestDto : CreateSponsorshipRequestDto
{
    public string? Remarks { get; set; }
}

public class ActionRemarksDto
{
    public string? Remarks { get; set; }
}

public class WorkflowHistoryDto
{
    public string Id { get; set; } = "";
    public string ActionByName { get; set; } = "";
    public string PreviousStatus { get; set; } = "";
    public string NewStatus { get; set; } = "";
    public string? Remarks { get; set; }
    public DateTime Timestamp { get; set; }
}
