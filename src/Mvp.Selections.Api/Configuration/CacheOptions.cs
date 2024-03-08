using System;

namespace Mvp.Selections.Api.Configuration
{
    public class CacheOptions
    {
        public const string Cache = "Cache";

        public string MvpUsersCacheKey { get; set; } = "MvpUsers";

        public TimeSpan MvpUsersCacheDuration { get; set; } = TimeSpan.FromHours(1);
    }
}
