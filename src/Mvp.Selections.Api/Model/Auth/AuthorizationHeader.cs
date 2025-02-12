using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Mvp.Selections.Api.Model.Auth;

public class AuthorizationHeader
{
    public const string AuthorizationHeaderKey = "Authorization";

    public string Scheme { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public static AuthorizationHeader? ParseFrom(IHeaderDictionary headers)
    {
        AuthorizationHeader? result = null;
        string? authHeaderValue;
        if (headers.TryGetValue(AuthorizationHeaderKey, out StringValues value) &&
            (authHeaderValue = value.FirstOrDefault()) != null)
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