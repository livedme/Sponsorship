using Microsoft.EntityFrameworkCore;
using Sponsorship.Application.Interfaces.Repositories;
using Sponsorship.Domain.Entities;
using Sponsorship.Domain.Enums;
using Sponsorship.Infrastructure.Data;

namespace Sponsorship.Infrastructure.Repositories;

public class SponsorshipRequestRepository : ISponsorshipRequestRepository
{
    private readonly AppDbContext _db;

    public SponsorshipRequestRepository(AppDbContext db) => _db = db;

    public Task<SponsorshipRequest?> GetByIdAsync(Guid id) =>
        _db.SponsorshipRequests.FindAsync(id).AsTask();

    public Task<SponsorshipRequest?> GetWithDetailsAsync(Guid id) =>
        _db.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistory)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<List<SponsorshipRequest>> GetAllWithDetailsAsync() =>
        await _db.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistory)
            .OrderByDescending(r => r.UpdatedAt)
            .ToListAsync();

    public async Task<List<SponsorshipRequest>> GetForCallerAsync(Guid callerId, string callerRole)
    {
        var query = _db.SponsorshipRequests
            .Include(r => r.SponsorshipType)
            .Include(r => r.WorkflowHistory)
            .AsQueryable();

        query = callerRole switch
        {
            "Requestor" => query.Where(r => r.RequestorId == callerId),
            _           => query   // Manager, FinanceAdmin, SystemAdmin see all requests
        };

        return await query.OrderByDescending(r => r.UpdatedAt).ToListAsync();
    }

    public Task AddAsync(SponsorshipRequest request)
    {
        _db.SponsorshipRequests.Add(request);
        return Task.CompletedTask;
    }
}
