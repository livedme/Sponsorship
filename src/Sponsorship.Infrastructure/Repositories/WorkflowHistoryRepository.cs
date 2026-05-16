using Microsoft.EntityFrameworkCore;
using Sponsorship.Application.Interfaces.Repositories;
using Sponsorship.Domain.Entities;
using Sponsorship.Infrastructure.Data;

namespace Sponsorship.Infrastructure.Repositories;

public class WorkflowHistoryRepository : IWorkflowHistoryRepository
{
    private readonly AppDbContext _db;

    public WorkflowHistoryRepository(AppDbContext db) => _db = db;

    public async Task<List<WorkflowHistory>> GetAllWithDetailsAsync() =>
        await _db.WorkflowHistories
            .Include(h => h.Request)
            .OrderByDescending(h => h.Timestamp)
            .ToListAsync();

    public Task AddAsync(WorkflowHistory history)
    {
        _db.WorkflowHistories.Add(history);
        return Task.CompletedTask;
    }
}
