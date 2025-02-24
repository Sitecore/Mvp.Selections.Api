using System.Security.Cryptography;
using System.Text;

namespace Mvp.Selections.Api.Extensions;

public static class StringExtensions
{
    public static string ToMD5Hash(this string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = MD5.HashData(inputBytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    public static string ToBase64(this string input, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        return Convert.ToBase64String(encoding.GetBytes(input));
    }

    public static string Format(this string input, params object?[] args)
    {
        return string.Format(input, args);
    }
}