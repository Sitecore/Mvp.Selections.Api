namespace Mvp.Selections.Client.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToQueryString(this object? obj, string key, bool isFirst = false)
        {
            string result = string.Empty;
            if (obj != null)
            {
                string start = isFirst ? "?" : "&";
                result = $"{start}{key}={obj}";
            }

            return result;
        }
    }
}
