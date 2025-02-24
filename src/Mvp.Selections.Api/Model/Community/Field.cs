using Newtonsoft.Json;

namespace Mvp.Selections.Api.Model.Community;

public class Field
{
    [JsonProperty("label")]
    public string? Label { get; set; }

    [JsonProperty("source_field")]
    public string? SourceField { get; set; }

    [JsonProperty("source_user_field")]
    public string? SourceUserField { get; set; }

    [JsonProperty("read_only")]
    public string? ReadOnly { get; set; }

    [JsonProperty("privacy")]
    public string? Privacy { get; set; }

    [JsonProperty("display")]
    public string? Display { get; set; }

    [JsonProperty("source_table")]
    public string? SourceTable { get; set; }

    [JsonProperty("value")]
    public string? Value { get; set; }
}