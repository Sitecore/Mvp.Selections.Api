using Newtonsoft.Json;

namespace Mvp.Selections.Api.Model.X;

public class ErrorList
{
    [JsonProperty("errors")]
    public List<Error> Errors { get; set; } = [];
}