using Microsoft.AspNetCore.Http;
using Mvp.Selections.Api.Extensions;

namespace Mvp.Selections.Api.Model.Request
{
    public class ListParameters
    {
        public const string PageQueryStringKey = "p";

        public const string PageSizeQueryStringKey = "ps";

        public ListParameters(HttpRequest req)
        {
            Page = req.Query.GetFirstValueOrDefault<int>(PageQueryStringKey);
            if (Page <= 0)
            {
                Page = 1;
            }

            PageSize = req.Query.GetFirstValueOrDefault<short>(PageSizeQueryStringKey);
            if (PageSize <= 0)
            {
                PageSize = 100;
            }
        }

        public int Page { get; set; }

        public short PageSize { get; set; }
    }
}
