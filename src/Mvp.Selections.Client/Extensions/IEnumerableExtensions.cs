using System.Web;

namespace Mvp.Selections.Client.Extensions
{
    // ReSharper disable once InconsistentNaming - This class extends the interface, not the type
    public static class IEnumerableExtensions
    {
        public static string ToQueryString<T>(this IEnumerable<T>? enumerable, string key, bool isFirst = false)
        {
            string result = string.Empty;
            if (enumerable != null)
            {
                string separator = isFirst ? "?" : "&";
                foreach (T obj in enumerable)
                {
                    result += $"{separator}{key}={HttpUtility.UrlEncode(obj?.ToString())}";
                    separator = "&";
                }
            }

            return result;
        }
    }
}
