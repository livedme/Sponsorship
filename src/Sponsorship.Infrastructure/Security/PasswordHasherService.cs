using Microsoft.AspNetCore.Identity;
using Sponsorship.Application.Interfaces;
using Sponsorship.Domain.Entities;
using IdentityPasswordHasher = Microsoft.AspNetCore.Identity.IPasswordHasher<Sponsorship.Domain.Entities.User>;

namespace Sponsorship.Infrastructure.Security;

/// <summary>
/// Adapts ASP.NET Identity's <see cref="IPasswordHasher{TUser}"/> to the application's <see cref="IPasswordHasher"/> interface.
/// </summary>
public class PasswordHasherService : IPasswordHasher
{
    private readonly IdentityPasswordHasher _identityHasher;

    public PasswordHasherService(IdentityPasswordHasher identityHasher)
    {
        _identityHasher = identityHasher;
    }

    public string Hash(string password) =>
        _identityHasher.HashPassword(null!, password);

    public bool Verify(string password, string hashedPassword) =>
        _identityHasher.VerifyHashedPassword(null!, hashedPassword, password)
            != PasswordVerificationResult.Failed;
}
