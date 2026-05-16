namespace Sponsorship.Application.DTOs;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string Token, string FullName, string Role, Guid UserId);

public record CurrentUserDto(Guid Id, string Email, string FullName, string Department, string Role);
