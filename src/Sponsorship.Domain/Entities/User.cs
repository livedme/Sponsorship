using Sponsorship.Domain.Enums;

namespace Sponsorship.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SponsorshipRequest> Requests { get; set; } = new List<SponsorshipRequest>();
    public ICollection<WorkflowHistory> WorkflowActions { get; set; } = new List<WorkflowHistory>();
}
