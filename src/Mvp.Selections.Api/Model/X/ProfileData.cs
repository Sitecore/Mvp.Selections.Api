using System;
using Newtonsoft.Json;

namespace Mvp.Selections.Api.Model.X
{
    public class ProfileData
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("username")]
        public string? UserName { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("profile_image_url")]
        public string? ProfileImage { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
