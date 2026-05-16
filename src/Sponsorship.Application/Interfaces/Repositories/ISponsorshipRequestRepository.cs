using Sponsorship.Domain.Entities;

namespace Sponsorship.Application.Interfaces.Repositories;

public interface ISponsorshipRequestRepository
{
    Task<SponsorshipRequest?> GetByIdAsync(Guid id);
    Task<SponsorshipRequest?> GetWithDetailsAsync(Guid id);
    Task<List<SponsorshipRequest>> GetAllWithDetailsAsync();
    Task<List<SponsorshipRequest>> GetForCallerAsync(Guid callerId, string callerRole);
    Task AddAsync(SponsorshipRequest request);
}
