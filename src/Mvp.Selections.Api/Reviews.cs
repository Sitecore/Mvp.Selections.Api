using System;
using System.Collections.Generic;
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
using Mvp.Selections.Api.Model.Auth;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api
{
    public class Reviews : Base<Reviews>
    {
        private readonly IReviewService _reviewService;

        public Reviews(ILogger<Reviews> logger, ISerializer serializer, IAuthService authService, IReviewService reviewService)
            : base(logger, serializer, authService)
        {
            _reviewService = reviewService;
        }

        [FunctionName("GetReview")]
        [OpenApiOperation("GetReview", "Reviews", "Admin", "Review")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Review))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/reviews/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Review }, async authResult =>
            {
                OperationResult<Review> getResult = await _reviewService.GetAsync(authResult.User, id);
                return ContentResult(getResult, ReviewsContractResolver.Instance);
            });
        }

        [FunctionName("GetAllReviewsForApplication")]
        [OpenApiOperation("GetAllReviewsForApplication", "Reviews", "Admin", "Review")]
        [OpenApiParameter("applicationId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Review>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAllForApplication(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/applications/{applicationId:Guid}/reviews")]
            HttpRequest req,
            Guid applicationId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Review }, async authResult =>
            {
                ListParameters lp = new (req);
                OperationResult<IList<Review>> getResult = await _reviewService.GetAllAsync(authResult.User, applicationId, lp.Page, lp.PageSize);
                return ContentResult(getResult, ReviewsContractResolver.Instance);
            });
        }

        [FunctionName("AddReview")]
        [OpenApiOperation("AddReview", "Reviews", "Admin", "Review")]
        [OpenApiParameter("applicationId", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(Review))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Review))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/applications/{applicationId:Guid}/reviews")]
            HttpRequest req,
            Guid applicationId)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Review }, async authResult =>
            {
                Review input = await Serializer.DeserializeAsync<Review>(req.Body);
                OperationResult<Review> addResult = await _reviewService.AddAsync(authResult.User, applicationId, input);
                return ContentResult(addResult, ReviewsContractResolver.Instance);
            });
        }

        [FunctionName("UpdateReview")]
        [OpenApiOperation("UpdateReview", "Reviews", "Admin", "Review")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(Review))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Review))]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/reviews/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin, Right.Review }, async authResult =>
            {
                Review input = await Serializer.DeserializeAsync<Review>(req.Body);
                OperationResult<Review> updateResult = await _reviewService.UpdateAsync(authResult.User, id, input);
                return ContentResult(updateResult, ReviewsContractResolver.Instance);
            });
        }

        [FunctionName("RemoveReview")]
        [OpenApiOperation("RemoveReview", "Reviews", "Admin", "Review")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(Guid), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.BadRequest, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/reviews/{id:Guid}")]
            HttpRequest req,
            Guid id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async authResult =>
            {
                OperationResult<Review> removeResult = await _reviewService.RemoveAsync(authResult.User, id);
                return ContentResult(removeResult);
            });
        }
    }
}
