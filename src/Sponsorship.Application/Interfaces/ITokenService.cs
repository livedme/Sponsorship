using Sponsorship.Domain.Entities;

namespace Sponsorship.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
