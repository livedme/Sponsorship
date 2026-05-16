using Sponsorship.Application.DTOs;

namespace Sponsorship.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<CurrentUserDto?> GetCurrentUserAsync(Guid userId);
}
