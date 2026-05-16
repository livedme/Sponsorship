using Sponsorship.Domain.Entities;

namespace Sponsorship.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateToken(User user);
}
