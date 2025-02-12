using Newtonsoft.Json;

namespace Mvp.Selections.Api.Model.X;

public class Token
{
    [JsonProperty("token_type")]
    public string? TokenType { get; set; }

    [JsonProperty("access_token")]
    public string? AccessToken { get; set; }
}