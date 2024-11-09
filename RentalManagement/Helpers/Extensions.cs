using System.Security.Cryptography;
using System.Text;

namespace RentalManagement.Helpers;
public static class Extensions
{
    /// <summary>
    /// Saltify a string using SHA256
    /// </summary>
    /// <param name="input">String to salt</param>
    /// <returns>Salted <paramref name="input"/></returns>
    public static string ToSHA256(this string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
