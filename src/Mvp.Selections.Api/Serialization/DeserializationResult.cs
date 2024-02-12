using System.Collections.Generic;

namespace Mvp.Selections.Api.Serialization
{
    public class DeserializationResult<T>
    {
        public T? Object { get; set; }

        public IList<string> PropertyKeys { get; set; } = new List<string>();
    }
}
