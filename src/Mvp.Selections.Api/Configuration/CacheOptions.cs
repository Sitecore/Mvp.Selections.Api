namespace Mvp.Selections.Api.Configuration
{
    public class CacheOptions
    {
        public const string Cache = "Cache";

        public string MvpProfilesCacheKey { get; set; } = "MvpUsers";

        public int MvpProfilesCacheDurationInSeconds { get; set; } = 3600;
    }
}
