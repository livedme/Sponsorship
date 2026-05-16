using Sponsorship.Domain.Enums;

namespace Sponsorship.Domain.Entities;

public class WorkflowHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid RequestId { get; set; }
    public SponsorshipRequest Request { get; set; } = null!;

    public Guid ActionById { get; set; }
    public User ActionBy { get; set; } = null!;
    public string ActionByName { get; set; } = string.Empty;

    public RequestStatus PreviousStatus { get; set; }
    public RequestStatus NewStatus { get; set; }
    public string? Remarks { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
