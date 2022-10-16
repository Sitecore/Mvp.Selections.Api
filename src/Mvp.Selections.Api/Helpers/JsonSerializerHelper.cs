using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Helpers
{
    public class JsonSerializerHelper : ISerializerHelper
    {
        private readonly JsonOptions _options;

        public JsonSerializerHelper(IOptionsSnapshot<JsonOptions> options)
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
