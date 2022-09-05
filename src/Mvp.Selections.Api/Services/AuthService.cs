using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Clients;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Api.Model.Auth;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly OktaClient _oktaClient;

        private readonly JwtSecurityTokenHandler _tokenHandler = new ();

        private readonly TokenOptions _tokenOptions;

        private readonly IUserRepository _userRepository;

        private readonly ICurrentUserNameProvider _currentUserNameProvider;

        public AuthService(OktaClient oktaClient, IOptions<TokenOptions> tokenOptions, IUserRepository userRepository, ICurrentUserNameProvider currentUserNameProvider)
        {
            _oktaClient = oktaClient;
            _tokenOptions = tokenOptions.Value;
            _userRepository = userRepository;
            _currentUserNameProvider = currentUserNameProvider;
        }

        public async Task<AuthResult> ValidateAsync(HttpRequest request, params Right[] rights)
        {
            AuthResult result = new ();
            AuthorizationHeader authHeader = AuthorizationHeader.ParseFrom(request.Headers);
            if (authHeader != null)
            {
                switch (authHeader.Scheme)
                {
                    case IAuthService.BearerScheme:
                        await ValidateBearerAsync(result, authHeader, rights);
                        break;
                    default:
                        result.Message = "Unsupported authorization scheme used";
                        result.StatusCode = HttpStatusCode.Unauthorized;
                        break;
                }
            }
            else
            {
                result.Message = "Missing authorization header";
                result.StatusCode = HttpStatusCode.Unauthorized;
            }

            return result;
        }

        private static void ValidateRights(AuthResult result, IEnumerable<Right> rights)
        {
            result.StatusCode = rights.Aggregate(false, (current, right) => current | result.User.HasRight(right))
                ? HttpStatusCode.OK
                : HttpStatusCode.Forbidden;
        }

        private async Task ValidateBearerAsync(AuthResult result, AuthorizationHeader authHeader, IEnumerable<Right> rights)
        {
            if (await _oktaClient.IsValidAsync(authHeader.Token))
            {
                if (_tokenHandler.CanReadToken(authHeader.Token))
                {
                    result.TokenUser = new OktaUser(_tokenHandler.ReadJwtToken(authHeader.Token), _tokenOptions);
                    result.User = await _userRepository.GetForAuthAsync(result.TokenUser.Identifier);
                    if (result.User != null)
                    {
                        ValidateRights(result, rights);
                        _currentUserNameProvider.UserName = result.User.Identifier;
                    }
                    else
                    {
                        result.Message = "User doesn't exist";
                        result.StatusCode = HttpStatusCode.Forbidden;
                    }
                }
                else
                {
                    result.Message = "Token could not be read";
                    result.StatusCode = HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                result.Message = "Token is invalid";
                result.StatusCode = HttpStatusCode.Forbidden;
            }
        }
    }
}
