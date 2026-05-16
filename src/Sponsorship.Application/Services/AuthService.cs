using Sponsorship.Application.DTOs;
using Sponsorship.Application.Interfaces;
using Sponsorship.Application.Interfaces.Repositories;

namespace Sponsorship.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;
    private readonly IRoleService _roles;

    public AuthService(IUserRepository users, IPasswordHasher hasher, ITokenService tokens, IRoleService roles)
    {
        _users = users;
        _hasher = hasher;
        _tokens = tokens;
        _roles = roles;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email);
        if (user == null || !_hasher.Verify(request.Password, user.PasswordHash!))
            return null;

        var token = await _tokens.GenerateToken(user);
        var roles = await _roles.GetRolesAsync(user);
        var role  = roles.FirstOrDefault() ?? string.Empty;
        return new LoginResponse(token, user.FullName, role, user.Id);
    }

    public async Task<CurrentUserDto?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user == null) return null;
        var roles = await _roles.GetRolesAsync(user);
        var role  = roles.FirstOrDefault() ?? string.Empty;
        return new CurrentUserDto(user.Id, user.Email!, user.FullName, user.Department, role);
    }
}
