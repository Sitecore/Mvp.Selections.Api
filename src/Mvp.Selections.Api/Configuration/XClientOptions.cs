using System;

namespace Mvp.Selections.Api.Configuration;

public class XClientOptions
{
    public const string XClient = "XClient";

    public string ApiKey { get; set; } = string.Empty;

    public string ApiSecret { get; set; } = string.Empty;

    public string BearerCacheKey { get; set; } = "XClientBearer";

    public TimeSpan BearerCacheSlidingExpirationTime { get; set; } = TimeSpan.FromHours(1);

    public Uri TokenEndpoint { get; set; } = new("https://api.twitter.com/oauth2/token");

    public Uri BaseAddress { get; set; } = new("https://api.twitter.com/");
}