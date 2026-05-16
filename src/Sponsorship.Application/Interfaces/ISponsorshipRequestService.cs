using Sponsorship.Application.DTOs;

namespace Sponsorship.Application.Interfaces;

public interface ISponsorshipRequestService
{
    Task<SponsorshipRequestDto> CreateDraftAsync(CreateSponsorshipRequestDto dto, Guid requestorId);
    Task<SponsorshipRequestDto> UpdateDraftAsync(Guid requestId, UpdateSponsorshipRequestDto dto, Guid requestorId);
    Task<SponsorshipRequestDto> SubmitAsync(Guid requestId, Guid requestorId);
    Task<SponsorshipRequestDto> CancelAsync(Guid requestId, Guid requestorId);
    Task<SponsorshipRequestDto> ManagerApproveAsync(Guid requestId, string? remarks, Guid managerId);
    Task<SponsorshipRequestDto> ManagerRejectAsync(Guid requestId, string? remarks, Guid managerId);
    Task<SponsorshipRequestDto> FinanceApproveAsync(Guid requestId, string? remarks, Guid financeId);
    Task<SponsorshipRequestDto> FinanceRejectAsync(Guid requestId, string? remarks, Guid financeId);
    Task<SponsorshipRequestDto?> GetByIdAsync(Guid requestId, Guid callerId, string callerRole);
    Task<List<SponsorshipRequestDto>> GetAllForCallerAsync(Guid callerId, string callerRole);
    Task<List<SponsorshipRequestDto>> GetAllAsync();
    Task<List<WorkflowHistoryDto>> GetAllWorkflowHistoryAsync();
    Task<List<SponsorshipTypeDto>> GetSponsorshipTypesAsync();
    Task<SponsorshipTypeDto> CreateSponsorshipTypeAsync(CreateSponsorshipTypeDto dto);
    Task<SponsorshipTypeDto> UpdateSponsorshipTypeAsync(int id, CreateSponsorshipTypeDto dto);
    Task DeleteSponsorshipTypeAsync(int id);
}
