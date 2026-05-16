using System.Security.Cryptography;

namespace Sponsorship.Infrastructure.Security;

/// <summary>
/// Simple PBKDF2-based password hashing (no external dependencies).
/// </summary>
public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public static string Hash(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        var hash = Pbkdf2(password, salt);
        var result = new byte[SaltSize + HashSize];
        Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
        Buffer.BlockCopy(hash, 0, result, SaltSize, HashSize);
        return Convert.ToBase64String(result);
    }

    public static bool Verify(string password, string hashedPassword)
    {
        var data = Convert.FromBase64String(hashedPassword);
        if (data.Length != SaltSize + HashSize) return false;

        var salt = new byte[SaltSize];
        Buffer.BlockCopy(data, 0, salt, 0, SaltSize);
        var storedHash = new byte[HashSize];
        Buffer.BlockCopy(data, SaltSize, storedHash, 0, HashSize);

        var computedHash = Pbkdf2(password, salt);
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }

    private static byte[] Pbkdf2(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password, salt, Iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(HashSize);
    }
}
