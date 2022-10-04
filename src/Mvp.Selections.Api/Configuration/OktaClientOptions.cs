using System;

namespace Mvp.Selections.Api.Configuration
{
    public class OktaClientOptions
    {
        public const string OktaClient = "OktaClient";

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public Uri ValidationEndpoint { get; set; }

        public string ValidIssuer { get; set; } = string.Empty;

        public short CacheDuration { get; set; } = 10;
    }
}
