using Sponsorship.Domain.Entities;

namespace Sponsorship.Application.Interfaces.Repositories;

public interface IWorkflowHistoryRepository
{
    Task<List<WorkflowHistory>> GetAllWithDetailsAsync();
    Task AddAsync(WorkflowHistory history);
}
