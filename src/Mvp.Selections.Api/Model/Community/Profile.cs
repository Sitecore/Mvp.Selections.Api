using Newtonsoft.Json;

namespace Mvp.Selections.Api.Model.Community
{
    public class Profile
    {
        [JsonProperty("profileId")]
        public string? ProfileId { get; set; }

        [JsonProperty("country")]
        public Field? Country { get; set; }

        [JsonProperty("title")]
        public Field? Title { get; set; }

        [JsonProperty("short_description")]
        public Field? ShortDescription { get; set; }

        [JsonProperty("display_name")]
        public Field? DisplayName { get; set; }

        [JsonProperty("company")]
        public Field? Company { get; set; }

        [JsonProperty("state")]
        public Field? State { get; set; }

        [JsonProperty("photo")]
        public Field? Photo { get; set; }

        [JsonProperty("city")]
        public Field? City { get; set; }
    }
}
