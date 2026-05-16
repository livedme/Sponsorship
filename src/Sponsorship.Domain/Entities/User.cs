using Microsoft.AspNetCore.Identity;

namespace Sponsorship.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public User()
    {
        Id = Guid.NewGuid();
    }

    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SponsorshipRequest> Requests { get; set; } = new List<SponsorshipRequest>();
    public ICollection<WorkflowHistory> WorkflowActions { get; set; } = new List<WorkflowHistory>();
}
