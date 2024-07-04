using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mvp.Selections.Api.Clients;
using Mvp.Selections.Api.Configuration;
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Api.Model.Auth;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Data;
using Mvp.Selections.Data.Repositories.Interfaces;
using Mvp.Selections.Domain;
using Mvp.Selections.Domain.Roles;

namespace Mvp.Selections.Api.Services
{
    public class AuthService(
        ILogger<AuthService> logger,
        OktaClient oktaClient,
        IOptions<TokenOptions> tokenOptions,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ICurrentUserNameProvider currentUserNameProvider)
        : IAuthService
    {
        private static readonly object _NewUserLock = new();

        private readonly JwtSecurityTokenHandler _tokenHandler = new();

        private readonly TokenOptions _tokenOptions = tokenOptions.Value;

        public async Task<AuthResult> ValidateAsync(HttpRequest request, params Right[] rights)
        {
            AuthResult result = new();
            AuthorizationHeader? authHeader = AuthorizationHeader.ParseFrom(request.Headers);
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
            result.StatusCode = rights.Aggregate(false, (current, right) => current | result.User?.HasRight(right) ?? false)
                ? HttpStatusCode.OK
                : HttpStatusCode.Forbidden;
        }

        private async Task ValidateBearerAsync(AuthResult result, AuthorizationHeader authHeader, IEnumerable<Right> rights)
        {
            if (await oktaClient.IsValidAsync(authHeader.Token))
            {
                if (_tokenHandler.CanReadToken(authHeader.Token))
                {
                    result.TokenUser = new OktaUser(_tokenHandler.ReadJwtToken(authHeader.Token), _tokenOptions);
                    result.User = await userRepository.GetForAuthAsync(result.TokenUser.Identifier);
                    if (result.User != null)
                    {
                        ValidateRights(result, rights);
                        currentUserNameProvider.UserName = result.User.Identifier;
                    }
                    else
                    {
                        Role? defaultCandidateRole = await roleRepository.GetAsync(Context.DefaultCandidateRoleId);
                        lock (_NewUserLock)
                        {
                            if (!userRepository.DoesUserExist(result.TokenUser.Identifier))
                            {
                                currentUserNameProvider.UserName = result.TokenUser.Identifier;
                                User newUser = new(Guid.Empty)
                                {
                                    Identifier = result.TokenUser.Identifier,
                                    Name = result.TokenUser?.Name ?? string.Empty,
                                    Email = result.TokenUser?.Email ?? string.Empty
                                };
                                if (defaultCandidateRole != null)
                                {
                                    newUser.Roles.Add(defaultCandidateRole);
                                }
                                else
                                {
                                    logger.LogCritical("The Default Candidate Role with Id '{DefaultCandidateRoleId}' could not be found!", Context.DefaultCandidateRoleId);
                                }

                                userRepository.Add(newUser);
                                userRepository.SaveChanges();
                                result.StatusCode = HttpStatusCode.OK;
                                result.User = newUser;
                            }
                            else
                            {
                                result.Message = "Concurrent new user requests";
                                result.StatusCode = HttpStatusCode.TooManyRequests;
                            }
                        }
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
