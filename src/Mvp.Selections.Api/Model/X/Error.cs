using Newtonsoft.Json;

namespace Mvp.Selections.Api.Model.X;

public class Error
{
    [JsonProperty("code")]
    public int? Code { get; set; }

    [JsonProperty("label")]
    public string? Label { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }
}