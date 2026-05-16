using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Sponsorship.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    protected string CurrentUserRole =>
        User.FindFirstValue(ClaimTypes.Role)!;
}
