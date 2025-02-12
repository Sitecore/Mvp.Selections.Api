using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.Interfaces;

public interface ISerializer
{
    string ContentType { get; }

    Task<T?> DeserializeAsync<T>(Stream stream);

    Task<DeserializationResult<T>> DeserializeAsync<T>(Stream stream, bool extractPropertyKeys);

    string Serialize(object? data, IContractResolver? contractResolver = null);
}