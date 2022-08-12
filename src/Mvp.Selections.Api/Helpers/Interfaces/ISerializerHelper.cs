using System.IO;
using System.Threading.Tasks;

namespace Mvp.Selections.Api.Helpers.Interfaces
{
    public interface ISerializerHelper
    {
        public string ContentType { get; }

        public Task<T> DeserializeAsync<T>(Stream stream);

        public string Serialize(object data);
    }
}
