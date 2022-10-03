using System;

namespace Mvp.Selections.Api.Configuration
{
    public class OktaClientOptions
    {
        public const string OktaClient = "OktaClient";

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public Uri ValidationEndpoint { get; set; } = new ("https://externalsitecore.oktapreview.com/oauth2/default/v1/introspect");

        public string ValidIssuer { get; set; } = "https://externalsitecore.oktapreview.com/oauth2/default";

        public short CacheDuration { get; set; } = 10;
    }
}
