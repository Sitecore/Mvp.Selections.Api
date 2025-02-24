using Mvp.Selections.Api.Model.Send;

namespace Mvp.Selections.Api.Configuration;

public class SendClientOptions
{
    public const string SendClient = "SendClient";

    public string ApiKeyQueryStringKey { get; set; } = "apikey";

    public string ApiKey { get; set; } = string.Empty;

    public Uri BaseAddress { get; set; } = new("https://api.sitecoresend.io"); // Alternative: https://gateway.services.moosend.com

    public Sender Sender { get; set; } = new();
}