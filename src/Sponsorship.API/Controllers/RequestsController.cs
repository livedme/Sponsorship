using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sponsorship.Application.DTOs;
using Sponsorship.Application.Interfaces;

namespace Sponsorship.API.Controllers;

[Authorize]
[Route("api/requests")]
public class RequestsController : BaseController
{
    private readonly ISponsorshipRequestService _service;

    public RequestsController(ISponsorshipRequestService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _service.GetAllForCallerAsync(CurrentUserId, CurrentUserRole);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id, CurrentUserId, CurrentUserRole);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [Authorize(Roles = "Requestor")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSponsorshipRequestDto dto)
    {
        var result = await _service.CreateDraftAsync(dto, CurrentUserId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize(Roles = "Requestor")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSponsorshipRequestDto dto)
        => Ok(await _service.UpdateDraftAsync(id, dto, CurrentUserId));

    [Authorize(Roles = "Requestor")]
    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(Guid id)
        => Ok(await _service.SubmitAsync(id, CurrentUserId));

    [Authorize(Roles = "Requestor")]
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
        => Ok(await _service.CancelAsync(id, CurrentUserId));

    [Authorize(Roles = "Manager")]
    [HttpPost("{id:guid}/manager-approve")]
    public async Task<IActionResult> ManagerApprove(Guid id, [FromBody] ActionRemarksDto dto)
        => Ok(await _service.ManagerApproveAsync(id, dto.Remarks, CurrentUserId));

    [Authorize(Roles = "Manager")]
    [HttpPost("{id:guid}/manager-reject")]
    public async Task<IActionResult> ManagerReject(Guid id, [FromBody] ActionRemarksDto dto)
        => Ok(await _service.ManagerRejectAsync(id, dto.Remarks, CurrentUserId));

    [Authorize(Roles = "FinanceAdmin")]
    [HttpPost("{id:guid}/finance-approve")]
    public async Task<IActionResult> FinanceApprove(Guid id, [FromBody] ActionRemarksDto dto)
        => Ok(await _service.FinanceApproveAsync(id, dto.Remarks, CurrentUserId));

    [Authorize(Roles = "FinanceAdmin")]
    [HttpPost("{id:guid}/finance-reject")]
    public async Task<IActionResult> FinanceReject(Guid id, [FromBody] ActionRemarksDto dto)
        => Ok(await _service.FinanceRejectAsync(id, dto.Remarks, CurrentUserId));
}
