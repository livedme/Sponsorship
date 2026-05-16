using Sponsorship.Domain.Entities;

namespace Sponsorship.Application.Interfaces;

public interface IRoleService
{
    Task<IList<string>> GetRolesAsync(User user);
    Task<bool> IsInRoleAsync(User user, string role);
}
