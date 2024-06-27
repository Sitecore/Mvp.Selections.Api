using System.Web;

namespace Mvp.Selections.Client.Extensions
{
    /// <summary>
    /// Extensions for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Creates a querystring representation of the <see cref="string"/>.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to convert.</param>
        /// <param name="key">Key of the query string.</param>
        /// <param name="isFirst">If true the first separator will be '?' instead of '&amp;'.</param>
        /// <returns>A querystring version of the <see cref="string"/>.</returns>
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
