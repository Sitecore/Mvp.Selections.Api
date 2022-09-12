using System;
using System.IO;
using System.Threading.Tasks;
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Helpers
{
    public class JsonSerializerHelper : ISerializerHelper
    {
        private static readonly JsonSerializerSettings Settings;

        static JsonSerializerHelper()
        {
            Settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new MvpSelectionsDomainSerializationBinder()
            };
            Settings.Converters.Add(new StringEnumConverter());
        }

        public string ContentType => "application/json";

        public async Task<T> DeserializeAsync<T>(Stream stream)
        {
            using StreamReader reader = new (stream);
            string streamContent = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(streamContent, Settings);
        }

        public string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data, Settings);
        }

        // NOTE [ILs] We MUST whitelist deserialization to prevent code execution attacks:
        // https://www.alphabot.com/security/blog/2017/net/How-to-configure-Json.NET-to-create-a-vulnerable-web-API.html
        private class MvpSelectionsDomainSerializationBinder : DefaultSerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type result = null;
                if (assemblyName.Equals(typeof(BaseEntity<>).Assembly.GetName().Name))
                {
                    result = base.BindToType(assemblyName, typeName);
                }

                return result;
            }
        }
    }
}
