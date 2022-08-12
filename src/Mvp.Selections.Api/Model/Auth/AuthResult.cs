using System.Net;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Model.Auth
{
    public class AuthResult
    {
        public OktaUser TokenUser { get; set; }

        public User User { get; set; }

        public Right UserRights { get; set; }

        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Forbidden;

        public string Message { get; set; }
    }
}
