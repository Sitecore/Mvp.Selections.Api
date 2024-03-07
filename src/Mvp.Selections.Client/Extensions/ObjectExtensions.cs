using System.Web;

namespace Mvp.Selections.Client.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToQueryString(this object? obj, string key, bool isFirst = false)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(obj?.ToString()))
            {
                string start = isFirst ? "?" : "&";
                result = $"{start}{key}={HttpUtility.UrlEncode(obj.ToString())}";
            }

            return result;
        }
    }
}
