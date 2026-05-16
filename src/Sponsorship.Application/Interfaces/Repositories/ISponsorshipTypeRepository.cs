using Sponsorship.Domain.Entities;

namespace Sponsorship.Application.Interfaces.Repositories;

public interface ISponsorshipTypeRepository
{
    Task<List<SponsorshipType>> GetAllAsync();
    Task<SponsorshipType?> GetByIdAsync(int id);
    Task AddAsync(SponsorshipType type);
    Task RemoveAsync(SponsorshipType type);
}
