using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sponsorship.Application.Interfaces;

namespace Sponsorship.API.Controllers;

[Authorize]
[Route("api/sponsorship-types")]
public class SponsorshipTypesController : BaseController
{
    private readonly ISponsorshipRequestService _service;

    public SponsorshipTypesController(ISponsorshipRequestService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _service.GetSponsorshipTypesAsync();
        return Ok(list);
    }
}
