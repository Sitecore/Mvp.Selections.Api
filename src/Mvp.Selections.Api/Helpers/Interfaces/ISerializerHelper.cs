using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace Mvp.Selections.Api.Helpers.Interfaces
{
    public interface ISerializerHelper
    {
        string ContentType { get; }

        Task<T> DeserializeAsync<T>(Stream stream);

        string Serialize(object data, IContractResolver contractResolver = null);
    }
}
