using Newtonsoft.Json;

namespace Mvp.Selections.Api.Model.X
{
    public class Profile
    {
        [JsonProperty("data")]
        public ProfileData? Data { get; set; }
    }
}
