using Sponsorship.Application.DTOs;
using Sponsorship.Application.Interfaces;
using Sponsorship.Application.Interfaces.Repositories;

namespace Sponsorship.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public AuthService(IUserRepository users, IPasswordHasher hasher, ITokenService tokens)
    {
        _users = users;
        _hasher = hasher;
        _tokens = tokens;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email);
        if (user == null || !_hasher.Verify(request.Password, user.PasswordHash))
            return null;

        var token = _tokens.GenerateToken(user);
        return new LoginResponse(token, user.FullName, user.Role.ToString(), user.Id);
    }

    public async Task<CurrentUserDto?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user == null) return null;
        return new CurrentUserDto(user.Id, user.Email, user.FullName, user.Department, user.Role.ToString());
    }
}
