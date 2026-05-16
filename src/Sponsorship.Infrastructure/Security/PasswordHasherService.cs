using Sponsorship.Application.Interfaces;

namespace Sponsorship.Infrastructure.Security;

/// <summary>
/// Adapts the static <see cref="PasswordHasher"/> utility to the <see cref="IPasswordHasher"/> interface for DI.
/// </summary>
public class PasswordHasherService : IPasswordHasher
{
    public string Hash(string password) => PasswordHasher.Hash(password);
    public bool Verify(string password, string hashedPassword) => PasswordHasher.Verify(password, hashedPassword);
}
