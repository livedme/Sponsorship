using Sponsorship.Application.DTOs;
using Sponsorship.Application.Interfaces;
using Sponsorship.Application.Interfaces.Repositories;
using Sponsorship.Domain.Entities;
using Sponsorship.Domain.Enums;
using Sponsorship.Domain.Exceptions;

namespace Sponsorship.Application.Services;

public class SponsorshipRequestService : ISponsorshipRequestService
{
    private readonly ISponsorshipRequestRepository _requests;
    private readonly ISponsorshipTypeRepository _types;
    private readonly IWorkflowHistoryRepository _history;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public SponsorshipRequestService(
        ISponsorshipRequestRepository requests,
        ISponsorshipTypeRepository types,
        IWorkflowHistoryRepository history,
        IUserRepository users,
        IUnitOfWork uow)
    {
        _requests = requests;
        _types = types;
        _history = history;
        _users = users;
        _uow = uow;
    }

    public async Task<SponsorshipRequestDto> CreateDraftAsync(CreateSponsorshipRequestDto dto, Guid requestorId)
    {
        var requestor = await _users.GetByIdAsync(requestorId)
            ?? throw new NotFoundException(nameof(User), requestorId);

        var request = new SponsorshipRequest
        {
            RequestTitle = dto.RequestTitle,
            RequestorId = requestorId,
            RequestorName = requestor.FullName,
            Department = dto.Department,
            SponsorshipTypeId = dto.SponsorshipTypeId,
            EventName = dto.EventName,
            EventDate = DateTime.SpecifyKind(dto.EventDate, DateTimeKind.Utc),
            RequestedAmount = dto.RequestedAmount,
            Purpose = dto.Purpose,
            ExpectedBusinessBenefit = dto.ExpectedBusinessBenefit,
            Remarks = dto.Remarks,
            SupportingDocumentUrl = dto.SupportingDocumentUrl,
            Status = RequestStatus.Draft
        };

        await _requests.AddAsync(request);
        await _history.AddAsync(BuildHistory(request.Id, requestorId, requestor.FullName,
            RequestStatus.Draft, RequestStatus.Draft, "Request created as draft."));
        await _uow.SaveChangesAsync();

        return MapToDto(await _requests.GetWithDetailsAsync(request.Id)
            ?? throw new NotFoundException(nameof(SponsorshipRequest), request.Id));
    }

    public async Task<SponsorshipRequestDto> UpdateDraftAsync(Guid requestId, UpdateSponsorshipRequestDto dto, Guid requestorId)
    {
        var request = await _requests.GetByIdAsync(requestId)
            ?? throw new NotFoundException(nameof(SponsorshipRequest), requestId);

        if (request.RequestorId != requestorId)
            throw new ForbiddenException("You do not own this request.");
        if (request.Status != RequestStatus.Draft)
            throw new InvalidOperationException("Only Draft requests can be edited.");

        request.RequestTitle = dto.RequestTitle;
        request.Department = dto.Department;
        request.SponsorshipTypeId = dto.SponsorshipTypeId;
        request.EventName = dto.EventName;
        request.EventDate = DateTime.SpecifyKind(dto.EventDate, DateTimeKind.Utc);
        request.RequestedAmount = dto.RequestedAmount;
        request.Purpose = dto.Purpose;
        request.ExpectedBusinessBenefit = dto.ExpectedBusinessBenefit;
        request.Remarks = dto.Remarks;
        request.SupportingDocumentUrl = dto.SupportingDocumentUrl;
        request.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync();

        return MapToDto(await _requests.GetWithDetailsAsync(requestId)
            ?? throw new NotFoundException(nameof(SponsorshipRequest), requestId));
    }

    public async Task<SponsorshipRequestDto> SubmitAsync(Guid requestId, Guid requestorId)
    {
        var (request, actor) = await LoadAsync(requestId, requestorId);
        if (request.RequestorId != requestorId)
            throw new ForbiddenException("You do not own this request.");
        if (request.Status != RequestStatus.Draft)
            throw new InvalidOperationException("Only Draft requests can be submitted.");

        await TransitionAsync(request, actor, RequestStatus.PendingManagerApproval, null);
        await _uow.SaveChangesAsync();
        return MapToDto(await _requests.GetWithDetailsAsync(requestId)
            ?? throw new NotFoundException(nameof(SponsorshipRequest), requestId));
    }

    public async Task<SponsorshipRequestDto> CancelAsync(Guid requestId, Guid requestorId)
    {
        var (request, actor) = await LoadAsync(requestId, requestorId);
        if (request.RequestorId != requestorId)
            throw new ForbiddenException("You do not own this request.");
        if (request.Status != RequestStatus.Draft && request.Status != RequestStatus.PendingManagerApproval)
            throw new InvalidOperationException("Request cannot be cancelled at this stage.");

        await TransitionAsync(request, actor, RequestStatus.Cancelled, "Cancelled by requestor.");
        await _uow.SaveChangesAsync();
        return MapToDto(await _requests.GetWithDetailsAsync(requestId)
            ?? throw new NotFoundException(nameof(SponsorshipRequest), requestId));
    }

    public async Task<SponsorshipRequestDto> ManagerApproveAsync(Guid requestId, string? remarks, Guid managerId)
    {
        var (request, actor) = await LoadAsync(requestId, managerId);
        if (actor.Role != UserRole.Manager)
            throw new ForbiddenException("Only Managers can perform this action.");
        if (request.Status != RequestStatus.PendingManagerApproval)
            throw new InvalidOperationException("Request is not pending manager approval.");

        await TransitionAsync(request, actor, RequestStatus.PendingFinanceReview, remarks);
        await _uow.SaveChangesAsync();
        return MapToDto(await _requests.GetWithDetailsAsync(requestId)
            ?? throw new NotFoundException(nameof(SponsorshipRequest), requestId));
    }

    public async Task<SponsorshipRequestDto> ManagerRejectAsync(Guid requestId, string? remarks, Guid managerId)
    {
        var (request, actor) = await LoadAsync(requestId, managerId);
        if (actor.Role != UserRole.Manager)
            throw new ForbiddenException("Only Managers can perform this action.");
        if (request.Status != RequestStatus.PendingManagerApproval)
            throw new InvalidOperationException("Request is not pending manager approval.");

        await TransitionAsync(request, actor, RequestStatus.Rejected, remarks);
        await _uow.SaveChangesAsync();
        return MapToDto(await _requests.GetWithDetailsAsync(requestId)
            ?? throw new NotFoundException(nameof(SponsorshipRequest), requestId));
    }

    public async Task<SponsorshipRequestDto> FinanceApproveAsync(Guid requestId, string? remarks, Guid financeId)
    {
        var (request, actor) = await LoadAsync(requestId, financeId);
        if (actor.Role != UserRole.FinanceAdmin)
            throw new ForbiddenException("Only Finance Admins can perform this action.");
        if (request.Status != RequestStatus.PendingFinanceReview)
            throw new InvalidOperationException("Request is not pending finance review.");

        await TransitionAsync(request, actor, RequestStatus.Approved, remarks);
        await _uow.SaveChangesAsync();
        return MapToDto(await _requests.GetWithDetailsAsync(requestId)
            ?? throw new NotFoundException(nameof(SponsorshipRequest), requestId));
    }

    public async Task<SponsorshipRequestDto> FinanceRejectAsync(Guid requestId, string? remarks, Guid financeId)
    {
        var (request, actor) = await LoadAsync(requestId, financeId);
        if (actor.Role != UserRole.FinanceAdmin)
            throw new ForbiddenException("Only Finance Admins can perform this action.");
        if (request.Status != RequestStatus.PendingFinanceReview)
            throw new InvalidOperationException("Request is not pending finance review.");

        await TransitionAsync(request, actor, RequestStatus.Rejected, remarks);
        await _uow.SaveChangesAsync();
        return MapToDto(await _requests.GetWithDetailsAsync(requestId)
            ?? throw new NotFoundException(nameof(SponsorshipRequest), requestId));
    }

    public async Task<SponsorshipRequestDto?> GetByIdAsync(Guid requestId, Guid callerId, string callerRole)
    {
        var r = await _requests.GetWithDetailsAsync(requestId);
        if (r == null) return null;
        if (callerRole == "Requestor" && r.RequestorId != callerId) return null;
        return MapToDto(r);
    }

    public async Task<List<SponsorshipRequestDto>> GetAllForCallerAsync(Guid callerId, string callerRole)
    {
        var list = await _requests.GetForCallerAsync(callerId, callerRole);
        return list.Select(MapToDto).ToList();
    }

    public async Task<List<SponsorshipRequestDto>> GetAllAsync()
    {
        var list = await _requests.GetAllWithDetailsAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<List<WorkflowHistoryDto>> GetAllWorkflowHistoryAsync()
    {
        var list = await _history.GetAllWithDetailsAsync();
        return list.Select(MapHistoryDto).ToList();
    }

    public async Task<List<SponsorshipTypeDto>> GetSponsorshipTypesAsync()
    {
        var list = await _types.GetAllAsync();
        return list.Select(t => new SponsorshipTypeDto { Id = t.Id, Name = t.Name }).ToList();
    }

    public async Task<SponsorshipTypeDto> CreateSponsorshipTypeAsync(CreateSponsorshipTypeDto dto)
    {
        var type = new SponsorshipType { Name = dto.Name };
        await _types.AddAsync(type);
        await _uow.SaveChangesAsync();
        return new SponsorshipTypeDto { Id = type.Id, Name = type.Name };
    }

    public async Task<SponsorshipTypeDto> UpdateSponsorshipTypeAsync(int id, CreateSponsorshipTypeDto dto)
    {
        var type = await _types.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(SponsorshipType), id);
        type.Name = dto.Name;
        await _uow.SaveChangesAsync();
        return new SponsorshipTypeDto { Id = type.Id, Name = type.Name };
    }

    public async Task DeleteSponsorshipTypeAsync(int id)
    {
        var type = await _types.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(SponsorshipType), id);
        await _types.RemoveAsync(type);
        await _uow.SaveChangesAsync();
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private async Task<(SponsorshipRequest request, User actor)> LoadAsync(Guid requestId, Guid actorId)
    {
        var request = await _requests.GetByIdAsync(requestId)
            ?? throw new NotFoundException(nameof(SponsorshipRequest), requestId);
        var actor = await _users.GetByIdAsync(actorId)
            ?? throw new NotFoundException(nameof(User), actorId);
        return (request, actor);
    }

    private async Task TransitionAsync(SponsorshipRequest request, User actor, RequestStatus newStatus, string? remarks)
    {
        var prev = request.Status;
        request.Status = newStatus;
        request.UpdatedAt = DateTime.UtcNow;
        await _history.AddAsync(BuildHistory(request.Id, actor.Id, actor.FullName, prev, newStatus, remarks));
    }

    private static WorkflowHistory BuildHistory(Guid requestId, Guid actorId, string actorName,
        RequestStatus prev, RequestStatus next, string? remarks) => new()
    {
        RequestId = requestId,
        ActionById = actorId,
        ActionByName = actorName,
        PreviousStatus = prev,
        NewStatus = next,
        Remarks = remarks,
        Timestamp = DateTime.UtcNow
    };

    private static SponsorshipRequestDto MapToDto(SponsorshipRequest r) => new()
    {
        Id = r.Id,
        RequestTitle = r.RequestTitle,
        RequestorId = r.RequestorId,
        RequestorName = r.RequestorName,
        Department = r.Department,
        SponsorshipTypeId = r.SponsorshipTypeId,
        SponsorshipTypeName = r.SponsorshipType?.Name ?? "",
        EventName = r.EventName,
        EventDate = r.EventDate,
        RequestedAmount = r.RequestedAmount,
        Purpose = r.Purpose,
        ExpectedBusinessBenefit = r.ExpectedBusinessBenefit,
        Remarks = r.Remarks,
        SupportingDocumentUrl = r.SupportingDocumentUrl,
        Status = r.Status.ToString(),
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
        WorkflowHistory = r.WorkflowHistory
            .OrderBy(h => h.Timestamp)
            .Select(MapHistoryDto)
            .ToList()
    };

    private static WorkflowHistoryDto MapHistoryDto(WorkflowHistory h) => new()
    {
        Id = h.Id,
        RequestId = h.RequestId,
        ActionByName = h.ActionByName,
        PreviousStatus = h.PreviousStatus.ToString(),
        NewStatus = h.NewStatus.ToString(),
        Remarks = h.Remarks,
        Timestamp = h.Timestamp
    };
}
