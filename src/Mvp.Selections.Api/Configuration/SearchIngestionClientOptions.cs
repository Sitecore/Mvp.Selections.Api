using System;

namespace Mvp.Selections.Api.Configuration
{
    public class SearchIngestionClientOptions
    {
        public const string SearchIngestionClient = "SearchIngestionClient";

        public string ApiKey { get; set; } = string.Empty;

        public Uri BaseAddress { get; set; } = new ("https://discover.sitecorecloud.io");

        public string Domain { get; set; } = string.Empty;

        public SearchIngestionSourceEntity MvpSourceEntity { get; set; } = new ();

        public string MvpDefaultImage { get; set; } = "https://mvp.sitecore.com/images/mvp-base-user-grey.png";

        public string MvpContentType { get; set; } = "Mvp";

        public string MvpUrlFormat { get; set; } = "https://mvp.sitecore.com/mvp/{0}";

        public class SearchIngestionSourceEntity
        {
            public string Source { get; set; } = string.Empty;

            public string Entity { get; set; } = "content";
        }
    }
}
