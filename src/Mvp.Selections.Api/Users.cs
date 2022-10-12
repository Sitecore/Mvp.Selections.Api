using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Mvp.Selections.Api.Helpers.Interfaces;
using Mvp.Selections.Api.Model.Auth;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class Users : Base<Users>
    {
        private readonly IUserService _userService;

        public Users(ILogger<Users> logger, ISerializerHelper serializer, IAuthService authService, IUserService userService)
            : base(logger, serializer, authService)
        {
            _userService = userService;
        }

        public static IEnumerable<User> CleanOutput(IEnumerable<User> users)
        {
            IEnumerable<User> result = Array.Empty<User>();
            if (users != null)
            {
                result = users.Select(CleanOutput);
            }

            return result;
        }

        public static User CleanOutput(User user)
        {
            User result = null;
            if (user != null)
            {
                result = CleanOutputInternal(user);
            }

            return result;
        }

        [FunctionName("GetCurrentUser")]
        [OpenApiOperation("GetCurrentUser", "Users")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(User))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> GetCurrent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/users/current")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Any);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    User user = await _userService.GetAsync(authResult.User.Id);
                    result = new ContentResult { Content = Serializer.Serialize(user), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
                }
                else
                {
                    result = new ContentResult { Content = authResult.Message, ContentType = PlainTextContentType, StatusCode = (int)authResult.StatusCode };
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                result = new ContentResult { Content = e.Message, ContentType = PlainTextContentType, StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return result;
        }

        [FunctionName("UpdateCurrentUser")]
        [OpenApiOperation("UpdateCurrentUser", "Users")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(User))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> UpdateCurrent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/users/current")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Any);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    User input = await Serializer.DeserializeAsync<User>(req.Body);
                    OperationResult<User> updateResult = await _userService.UpdateAsync(authResult.User.Id, input);
                    result = updateResult.StatusCode == HttpStatusCode.OK
                        ? new ContentResult
                        {
                            Content = Serializer.Serialize(updateResult.Result),
                            ContentType = Serializer.ContentType,
                            StatusCode = (int)HttpStatusCode.OK
                        }
                        : new ContentResult
                        {
                            Content = string.Join(Environment.NewLine, updateResult.Messages),
                            ContentType = PlainTextContentType,
                            StatusCode = (int)updateResult.StatusCode
                        };
                }
                else
                {
                    result = new ContentResult { Content = authResult.Message, ContentType = PlainTextContentType, StatusCode = (int)authResult.StatusCode };
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                result = new ContentResult { Content = e.Message, ContentType = PlainTextContentType, StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return result;
        }

        [FunctionName("GetUser")]
        [OpenApiOperation("GetUser", "Users", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(User))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/users/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    User user = await _userService.GetAsync(id);
                    result = new ContentResult { Content = Serializer.Serialize(user), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
                }
                else
                {
                    result = new ContentResult { Content = authResult.Message, ContentType = PlainTextContentType, StatusCode = (int)authResult.StatusCode };
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                result = new ContentResult { Content = e.Message, ContentType = PlainTextContentType, StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return result;
        }

        [FunctionName("GetAllUsers")]
        [OpenApiOperation("GetAllUsers", "Users", "Admin")]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<User>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/users")]
            HttpRequest req)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin);
                if (authResult.TokenUser != null)
                {
                    ListParameters lp = new (req);
                    IList<User> users = await _userService.GetAllAsync(lp.Page, lp.PageSize);
                    result = new ContentResult { Content = Serializer.Serialize(users), ContentType = Serializer.ContentType, StatusCode = (int)HttpStatusCode.OK };
                }
                else
                {
                    result = new ContentResult { Content = authResult.Message, ContentType = PlainTextContentType, StatusCode = (int)authResult.StatusCode };
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                result = new ContentResult { Content = e.Message, ContentType = PlainTextContentType, StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return result;
        }

        [FunctionName("UpdateUser")]
        [OpenApiOperation("UpdateUser", "Users", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(User))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(User))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/users/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            IActionResult result;
            try
            {
                AuthResult authResult = await AuthService.ValidateAsync(req, Right.Admin);
                if (authResult.StatusCode == HttpStatusCode.OK)
                {
                    User input = await Serializer.DeserializeAsync<User>(req.Body);
                    OperationResult<User> updateResult = await _userService.UpdateAsync(id, input);
                    result = updateResult.StatusCode == HttpStatusCode.OK
                        ? new ContentResult
                        {
                            Content = Serializer.Serialize(updateResult.Result),
                            ContentType = Serializer.ContentType,
                            StatusCode = (int)HttpStatusCode.OK
                        }
                        : new ContentResult
                        {
                            Content = string.Join(Environment.NewLine, updateResult.Messages),
                            ContentType = PlainTextContentType,
                            StatusCode = (int)updateResult.StatusCode
                        };
                }
                else
                {
                    result = new ContentResult { Content = authResult.Message, ContentType = PlainTextContentType, StatusCode = (int)authResult.StatusCode };
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                result = new ContentResult { Content = e.Message, ContentType = PlainTextContentType, StatusCode = (int)HttpStatusCode.InternalServerError };
            }

            return result;
        }

        private static User CleanOutputInternal(User user)
        {
            User result = new (user.Id)
            {
                CreatedBy = user.CreatedBy,
                CreatedOn = user.CreatedOn,
                ModifiedBy = user.ModifiedBy,
                ModifiedOn = user.ModifiedOn,
                Name = user.Name,
                Reviews = null!,
                Titles = null!,
                Email = user.Email,
                Applications = null!,
                Consents = null!,
                Identifier = user.Identifier,
                ImageType = user.ImageType,
                Mentors = null!
            };

            foreach (Role role in user.Roles)
            {
                Role outputRole;
                if (role is SystemRole systemRole)
                {
                    outputRole = new SystemRole(systemRole.Id)
                    {
                        CreatedBy = systemRole.CreatedBy,
                        CreatedOn = systemRole.CreatedOn,
                        ModifiedBy = systemRole.ModifiedBy,
                        ModifiedOn = systemRole.ModifiedOn,
                        Name = systemRole.Name,
                        Rights = systemRole.Rights,
                        Users = null!
                    };
                }
                else if (role is SelectionRole selectionRole)
                {
                    outputRole = new SelectionRole(selectionRole.Id)
                    {
                        CreatedBy = selectionRole.CreatedBy,
                        CreatedOn = selectionRole.CreatedOn,
                        ModifiedBy = selectionRole.ModifiedBy,
                        ModifiedOn = selectionRole.ModifiedOn,
                        Name = selectionRole.Name,
                        Users = null!
                    };

                    if (selectionRole.Region != null)
                    {
                        ((SelectionRole)outputRole).Region = new Region(selectionRole.Region.Id)
                        {
                            CreatedBy = selectionRole.Region.CreatedBy,
                            CreatedOn = selectionRole.Region.CreatedOn,
                            ModifiedBy = selectionRole.Region.ModifiedBy,
                            ModifiedOn = selectionRole.Region.ModifiedOn,
                            Countries = null!,
                            Name = selectionRole.Region.Name
                        };
                    }

                    if (selectionRole.Selection != null)
                    {
                        ((SelectionRole)outputRole).Selection = new Selection(selectionRole.Selection.Id)
                        {
                            CreatedBy = selectionRole.Selection.CreatedBy,
                            CreatedOn = selectionRole.Selection.CreatedOn,
                            ModifiedBy = selectionRole.Selection.ModifiedBy,
                            ModifiedOn = selectionRole.Selection.ModifiedOn,
                            Titles = null!,
                            Year = selectionRole.Selection.Year,
                            ApplicationsEnd = selectionRole.Selection.ApplicationsEnd,
                            ApplicationsStart = selectionRole.Selection.ApplicationsStart,
                            ApplicationsActive = selectionRole.Selection.ApplicationsActive,
                            ReviewsActive = selectionRole.Selection.ReviewsActive,
                            ReviewsEnd = selectionRole.Selection.ReviewsEnd,
                            ReviewsStart = selectionRole.Selection.ReviewsStart
                        };
                    }

                    if (selectionRole.Country != null)
                    {
                        ((SelectionRole)outputRole).Country = new Country(selectionRole.Country.Id)
                        {
                            CreatedBy = selectionRole.Country.CreatedBy,
                            CreatedOn = selectionRole.Country.CreatedOn,
                            ModifiedBy = selectionRole.Country.ModifiedBy,
                            ModifiedOn = selectionRole.Country.ModifiedOn,
                            Name = selectionRole.Country.Name,
                            Users = null!
                        };
                    }

                    if (selectionRole.MvpType != null)
                    {
                        ((SelectionRole)outputRole).MvpType = new MvpType(selectionRole.MvpType.Id)
                        {
                            CreatedBy = selectionRole.MvpType.CreatedBy,
                            CreatedOn = selectionRole.MvpType.CreatedOn,
                            ModifiedBy = selectionRole.MvpType.ModifiedBy,
                            ModifiedOn = selectionRole.MvpType.ModifiedOn,
                            Name = selectionRole.MvpType.Name
                        };
                    }

                    if (selectionRole.Application != null)
                    {
                        ((SelectionRole)outputRole).Application = new Application(selectionRole.Application.Id)
                        {
                            CreatedBy = selectionRole.Application.CreatedBy,
                            CreatedOn = selectionRole.Application.CreatedOn,
                            ModifiedBy = selectionRole.Application.ModifiedBy,
                            ModifiedOn = selectionRole.Application.ModifiedOn,
                            Status = selectionRole.Application.Status,
                            Contributions = null!,
                            Reviews = null!
                        };
                    }
                }
                else
                {
                    outputRole = null;
                }

                result.Roles.Add(outputRole);
            }

            if (user.Country != null)
            {
                result.Country = new Country(user.Country.Id)
                {
                    CreatedBy = user.Country.CreatedBy,
                    CreatedOn = user.Country.CreatedOn,
                    ModifiedBy = user.Country.ModifiedBy,
                    ModifiedOn = user.Country.ModifiedOn,
                    Name = user.Country.Name,
                    Users = null!
                };

                if (user.Country.Region != null)
                {
                    result.Country.Region = new Region(user.Country.Region.Id)
                    {
                        CreatedBy = user.Country.Region.CreatedBy,
                        CreatedOn = user.Country.Region.CreatedOn,
                        ModifiedBy = user.Country.Region.ModifiedBy,
                        ModifiedOn = user.Country.Region.ModifiedOn,
                        Countries = null!,
                        Name = user.Country.Region.Name
                    };
                }
            }

            return result;
        }
    }
}
