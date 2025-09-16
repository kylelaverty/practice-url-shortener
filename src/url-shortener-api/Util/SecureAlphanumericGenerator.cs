using System.Security.Cryptography;
using System.Text;

namespace Url.Shortener.Api.Util;

public class SecureAlphanumericGenerator
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static string GenerateSecureRandomAlphanumeric(int length)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes); // Fills the byte array with cryptographically strong random bytes

        var sb = new StringBuilder(length);
        foreach (var b in bytes)
        {
            // Map the random byte to an index within the 'chars' string
            _ = sb.Append(Chars[b % Chars.Length]);
        }
        return sb.ToString();
    }
}