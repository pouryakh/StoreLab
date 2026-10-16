using Application.Interfaces.Security;
using System.Security.Cryptography;

namespace Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return $"PBKDF2|1|{Iterations}|{Convert.ToBase64String(salt)}|{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string passwordHash)
    {
        string[] parts = passwordHash.Split('|');

        if (parts.Length != 5)
            return false;

        string algorithm = parts[0];
        int version = int.Parse(parts[1]);
        int iterations = int.Parse(parts[2]);
        byte[] salt = Convert.FromBase64String(parts[3]);
        byte[] storedHash = Convert.FromBase64String(parts[4]);

        if (algorithm != "PBKDF2" || version != 1)
            return false;

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            storedHash.Length);

        return CryptographicOperations.FixedTimeEquals(
            hash,
            storedHash);
    }
}