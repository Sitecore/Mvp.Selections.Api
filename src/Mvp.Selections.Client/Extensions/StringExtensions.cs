using System.Web;

namespace Mvp.Selections.Client.Extensions
{
    public static class StringExtensions
    {
        public static string ToQueryString(this string? str, string key, bool isFirst = false)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(str))
            {
                string start = isFirst ? "?" : "&";
                result = $"{start}{key}={HttpUtility.UrlEncode(str)}";
            }

            return result;
        }
    }
}
