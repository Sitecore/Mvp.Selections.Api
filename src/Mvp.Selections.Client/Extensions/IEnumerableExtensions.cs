using System.Web;

namespace Mvp.Selections.Client.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IEnumerable{T}"/>.
    /// </summary>
    // ReSharper disable once InconsistentNaming - This class extends the interface, not the type
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Creates a querystring out of an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="enumerable">Enumerable to convert.</param>
        /// <param name="key">Key of the query string.</param>
        /// <param name="isFirst">If true the first separator will be '?' instead of '&amp;'.</param>
        /// <returns>A querystring version of the <see cref="IEnumerable{T}"/>.</returns>
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
