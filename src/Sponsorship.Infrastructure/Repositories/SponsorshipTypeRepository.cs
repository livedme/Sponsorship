using Microsoft.EntityFrameworkCore;
using Sponsorship.Application.Interfaces.Repositories;
using Sponsorship.Domain.Entities;
using Sponsorship.Infrastructure.Data;

namespace Sponsorship.Infrastructure.Repositories;

public class SponsorshipTypeRepository : ISponsorshipTypeRepository
{
    private readonly AppDbContext _db;

    public SponsorshipTypeRepository(AppDbContext db) => _db = db;

    public Task<List<SponsorshipType>> GetAllAsync() =>
        _db.SponsorshipTypes.OrderBy(t => t.Name).ToListAsync();

    public Task<SponsorshipType?> GetByIdAsync(int id) =>
        _db.SponsorshipTypes.FindAsync(id).AsTask();

    public Task AddAsync(SponsorshipType type)
    {
        _db.SponsorshipTypes.Add(type);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(SponsorshipType type)
    {
        _db.SponsorshipTypes.Remove(type);
        return Task.CompletedTask;
    }
}
