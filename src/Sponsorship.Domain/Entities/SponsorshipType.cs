namespace Sponsorship.Domain.Entities;

public class SponsorshipType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<SponsorshipRequest> Requests { get; set; } = new List<SponsorshipRequest>();
}
