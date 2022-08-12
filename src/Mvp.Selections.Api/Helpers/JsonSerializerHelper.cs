using System.IO;
using System.Threading.Tasks;
using Mvp.Selections.Api.Helpers.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Helpers
{
    public class JsonSerializerHelper : ISerializerHelper
    {
        private static readonly JsonSerializerSettings Settings = new ()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public string ContentType => "application/json";

        public async Task<T> DeserializeAsync<T>(Stream stream)
        {
            using StreamReader reader = new (stream);
            string streamContent = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(streamContent);
        }

        public string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data, Settings);
        }
    }
}
