using System.Collections.Generic;
using System.Net;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Model.Applications
{
    public class AddResult
    {
        public Application Application { get; set; }

        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;

        public IList<string> Messages { get; } = new List<string>();
    }
}
