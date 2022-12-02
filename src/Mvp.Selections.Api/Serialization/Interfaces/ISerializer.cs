using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Serialization.Interfaces
{
    public interface ISerializer
    {
        string ContentType { get; }

        Task<T> DeserializeAsync<T>(Stream stream);

        string Serialize(object data, IContractResolver contractResolver = null);
    }
}
