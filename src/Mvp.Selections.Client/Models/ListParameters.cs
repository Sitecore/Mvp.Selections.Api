namespace Mvp.Selections.Client.Models
{
    public class ListParameters
    {
        public const string PageQueryStringKey = "p";

        public const string PageSizeQueryStringKey = "ps";

        public int Page { get; set; } = 1;

        public short PageSize { get; set; } = 100;

        public string ToQueryString(bool isFirst = false)
        {
            string start = isFirst ? "?" : "&";
            return $"{start}{PageQueryStringKey}={Page}&{PageSizeQueryStringKey}={PageSize}";
        }
    }
}
