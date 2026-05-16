using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sponsorship.Application.DTOs;
using Sponsorship.Application.Interfaces;

namespace Sponsorship.API.Controllers;

[Authorize(Roles = "SystemAdmin")]
[Route("api/admin")]
public class AdminController : BaseController
{
    private readonly ISponsorshipRequestService _service;

    public AdminController(ISponsorshipRequestService service)
    {
        _service = service;
    }

    [HttpGet("requests")]
    public async Task<IActionResult> GetAllRequests()
    {
        var list = await _service.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("workflow-history")]
    public async Task<IActionResult> GetWorkflowHistory()
    {
        var list = await _service.GetAllWorkflowHistoryAsync();
        return Ok(list);
    }

    [HttpGet("sponsorship-types")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSponsorshipTypes()
    {
        var list = await _service.GetSponsorshipTypesAsync();
        return Ok(list);
    }

    [HttpPost("sponsorship-types")]
    public async Task<IActionResult> CreateSponsorshipType([FromBody] CreateSponsorshipTypeDto dto)
    {
        var result = await _service.CreateSponsorshipTypeAsync(dto);
        return Ok(result);
    }

    [HttpPut("sponsorship-types/{id:int}")]
    public async Task<IActionResult> UpdateSponsorshipType(int id, [FromBody] CreateSponsorshipTypeDto dto)
        => Ok(await _service.UpdateSponsorshipTypeAsync(id, dto));

    [HttpDelete("sponsorship-types/{id:int}")]
    public async Task<IActionResult> DeleteSponsorshipType(int id)
    {
        await _service.DeleteSponsorshipTypeAsync(id);
        return NoContent();
    }
}
