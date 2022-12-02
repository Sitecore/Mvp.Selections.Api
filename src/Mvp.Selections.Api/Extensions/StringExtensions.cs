using System;
using System.Security.Cryptography;
using System.Text;

namespace Mvp.Selections.Api.Extensions
{
    public static class StringExtensions
    {
        public static string ToMD5Hash(this string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = MD5.HashData(inputBytes);

            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
    }
}
