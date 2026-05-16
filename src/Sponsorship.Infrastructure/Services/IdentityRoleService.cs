using Microsoft.AspNetCore.Identity;
using Sponsorship.Application.Interfaces;
using Sponsorship.Domain.Entities;

namespace Sponsorship.Infrastructure.Services;

public class IdentityRoleService : IRoleService
{
    private readonly UserManager<User> _userManager;

    public IdentityRoleService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public Task<IList<string>> GetRolesAsync(User user) =>
        _userManager.GetRolesAsync(user);

    public Task<bool> IsInRoleAsync(User user, string role) =>
        _userManager.IsInRoleAsync(user, role);
}
