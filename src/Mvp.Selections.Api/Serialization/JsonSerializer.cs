using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Extensions;
using Mvp.Selections.Api.Serialization.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization;

public class JsonSerializer(ILogger<JsonSerializer> logger, IOptionsSnapshot<JsonOptions> options)
    : ISerializer
{
    private readonly JsonOptions _options = options.Value;

    public string ContentType => "application/json";

    public async Task<T?> DeserializeAsync<T>(Stream stream)
    {
        using StreamReader reader = new(stream);
        string streamContent = await reader.ReadToEndAsync();
        return JsonConvert.DeserializeObject<T>(streamContent, _options.JsonSerializerSettings);
    }

    public async Task<DeserializationResult<T>> DeserializeAsync<T>(Stream stream, bool extractPropertyKeys)
    {
        DeserializationResult<T> result = new();
        using StreamReader reader = new(stream);
        string streamContent = await reader.ReadToEndAsync();
        if (extractPropertyKeys)
        {
            using JsonDocument document = JsonDocument.Parse(streamContent);
            if (document.RootElement.ValueKind == JsonValueKind.Object)
            {
                result.PropertyKeys.AddRange(document.RootElement.EnumerateObject().Select(property => property.Name));
            }
        }

        result.Object = JsonConvert.DeserializeObject<T>(streamContent, _options.JsonSerializerSettings);
        return result;
    }

    public string Serialize(object? data, IContractResolver? contractResolver = null)
    {
#if DEBUG
        Stopwatch timer = Stopwatch.StartNew();
#endif
        if (contractResolver != null)
        {
            _options.JsonSerializerSettings.ContractResolver = contractResolver;
        }

        string result = JsonConvert.SerializeObject(data, _options.JsonSerializerSettings);
#if DEBUG
        timer.Stop();
        logger.LogDebug($"Serialized '{data?.GetType().AssemblyQualifiedName}' in {timer.ElapsedMilliseconds}ms.");
#endif

        return result;
    }
}