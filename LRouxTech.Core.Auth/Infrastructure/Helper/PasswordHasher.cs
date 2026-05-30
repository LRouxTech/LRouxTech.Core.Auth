using System.Security.Cryptography;
using System.Text;

namespace LRouxTech.Core.Auth.Infrastructure.Helper;

public static class PasswordHasher
{
    /// <summary>
    /// Hashes a password into a single byte array.
    /// </summary>
    public static byte[] HashPassword(string password)
    {
        string hashedPasswordString = BCrypt.Net.BCrypt.HashPassword(password);
        
        return Encoding.UTF8.GetBytes(hashedPasswordString);
    }

    /// <summary>
    /// Verifies the password against the stored byte array.
    /// </summary>
    public static bool VerifyPassword(string password, byte[] storedHash)
    {
        string hashedPasswordString = Encoding.UTF8.GetString(storedHash);
        
        return BCrypt.Net.BCrypt.Verify(password, hashedPasswordString);
    }
}