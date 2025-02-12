namespace Mvp.Selections.Api.Configuration;

public class OktaClientOptions
{
    public const string OktaClient = "OktaClient";

    public const string InvalidEndpoint = "http://localhost";

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public Uri ValidationEndpoint { get; set; } = new(InvalidEndpoint);

    public string ValidIssuer { get; set; } = string.Empty;

    public short CacheDuration { get; set; } = 10;
}