namespace Sponsorship.Application.DTOs;

public class WorkflowHistoryDto
{
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public string RequestTitle { get; set; } = string.Empty;
    public string ActionByName { get; set; } = string.Empty;
    public string PreviousStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public DateTime Timestamp { get; set; }
}

public class SponsorshipTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateSponsorshipTypeDto
{
    public string Name { get; set; } = string.Empty;
}
