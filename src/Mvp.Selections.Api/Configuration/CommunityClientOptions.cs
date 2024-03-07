using System;

namespace Mvp.Selections.Api.Configuration
{
    public class CommunityClientOptions
    {
        public const string CommunityClient = "CommunityClient";

        public Uri BaseAddress { get; set; } = new ("https://community.sitecore.com/");
    }
}
