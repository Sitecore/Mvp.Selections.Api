using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Mvp.Selections.Api.Model.Auth
{
    public class AuthorizationHeader
    {
        public const string AuthorizationHeaderKey = "Authorization";

        public string Scheme { get; set; }

        public string Token { get; set; }

        public static AuthorizationHeader ParseFrom(IHeaderDictionary headers)
        {
            AuthorizationHeader result = null;
            string authHeaderValue;
            if (headers.ContainsKey(AuthorizationHeaderKey) &&
                (authHeaderValue = headers[AuthorizationHeaderKey].FirstOrDefault()) != null)
            {
                string[] authHeader = authHeaderValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (authHeader.Length == 2)
                {
                    result = new AuthorizationHeader
                    {
                        Scheme = authHeader[0],
                        Token = authHeader[1]
                    };
                }
            }

            return result;
        }
    }
}
