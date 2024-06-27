using System.Web;

namespace Mvp.Selections.Client.Extensions
{
    /// <summary>
    /// Extensions for <see cref="object"/>.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Creates a querystring representation of the <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to convert.</param>
        /// <param name="key">Key of the query string.</param>
        /// <param name="isFirst">If true the first separator will be '?' instead of '&amp;'.</param>
        /// <returns>A querystring version of the <see cref="object"/>.</returns>
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
