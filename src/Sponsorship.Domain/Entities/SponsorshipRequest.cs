using Sponsorship.Domain.Enums;

namespace Sponsorship.Domain.Entities;

public class SponsorshipRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string RequestTitle { get; set; } = string.Empty;

    public Guid RequestorId { get; set; }
    public User Requestor { get; set; } = null!;
    public string RequestorName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;

    public int SponsorshipTypeId { get; set; }
    public SponsorshipType SponsorshipType { get; set; } = null!;

    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? ExpectedBusinessBenefit { get; set; }
    public string? Remarks { get; set; }
    public string? SupportingDocumentUrl { get; set; }

    public RequestStatus Status { get; set; } = RequestStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<WorkflowHistory> WorkflowHistory { get; set; } = new List<WorkflowHistory>();
}
