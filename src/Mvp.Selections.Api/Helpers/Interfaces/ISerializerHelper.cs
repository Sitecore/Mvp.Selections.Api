using System.IO;
using System.Threading.Tasks;

namespace Mvp.Selections.Api.Helpers.Interfaces
{
    public interface ISerializerHelper
    {
        string ContentType { get; }

        Task<T> DeserializeAsync<T>(Stream stream);

        string Serialize(object data);
    }
}
