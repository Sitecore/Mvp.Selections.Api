using System.Collections.Generic;
using System.Net;

namespace Mvp.Selections.Api.Model.Request
{
    public class OperationResult<T>
        where T : class
    {
        public T? Result { get; set; }

        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;

        public IList<string> Messages { get; } = new List<string>();
    }
}
