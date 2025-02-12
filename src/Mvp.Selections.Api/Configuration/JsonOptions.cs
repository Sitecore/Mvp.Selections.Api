using Mvp.Selections.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Configuration;

public class JsonOptions
{
    public const string Json = "Json";

    public JsonOptions()
    {
        JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new MvpSelectionsDomainSerializationBinder()
        };
        JsonSerializerSettings.Converters.Add(new StringEnumConverter());
    }

    public JsonSerializerSettings JsonSerializerSettings { get; set; }

    // NOTE [ILs] We MUST whitelist deserialization to prevent code execution attacks:
    // https://www.alphabot.com/security/blog/2017/net/How-to-configure-Json.NET-to-create-a-vulnerable-web-API.html
    private class MvpSelectionsDomainSerializationBinder : DefaultSerializationBinder
    {
        public override Type BindToType(string? assemblyName, string typeName)
        {
            Type? result = null;
            if (assemblyName?.Equals(typeof(BaseEntity<>).Assembly.GetName().Name) ?? false)
            {
                result = base.BindToType(assemblyName, typeName);
            }

            return result!;
        }
    }
}