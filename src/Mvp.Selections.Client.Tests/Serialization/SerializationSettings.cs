using Mvp.Selections.Client.Serialization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Mvp.Selections.Client.Tests.Serialization
{
    public static class SerializationSettings
    {
        public static JsonSerializerOptions GetOptions()
        {
            JsonSerializerOptions result = new ()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            result.Converters.Add(new JsonStringEnumConverter());
            result.Converters.Add(new RoleConverter());

            return result;
        }
    }
}
