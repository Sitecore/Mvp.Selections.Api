using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Serialization.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization
{
    public class JsonSerializer : ISerializer
    {
        private readonly JsonOptions _options;

        public JsonSerializer(IOptionsSnapshot<JsonOptions> options)
        {
            _options = options.Value;
        }

        public string ContentType => "application/json";

        public async Task<T> DeserializeAsync<T>(Stream stream)
        {
            using StreamReader reader = new (stream);
            string streamContent = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(streamContent, _options.JsonSerializerSettings);
        }

        public string Serialize(object data, IContractResolver contractResolver = null)
        {
            if (contractResolver != null)
            {
                _options.JsonSerializerSettings.ContractResolver = contractResolver;
            }

            return JsonConvert.SerializeObject(data, _options.JsonSerializerSettings);
        }
    }
}
