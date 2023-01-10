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
    public class Products : Base<Products>
    {
        private readonly IProductService _productService;

        public Products(ILogger<Products> logger, ISerializer serializer, IAuthService authService, IProductService productService)
            : base(logger, serializer, authService)
        {
            _productService = productService;
        }

        [FunctionName("GetProduct")]
        [OpenApiOperation("GetProduct", "Products")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(int), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Product))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/products/{id:int}")]
            HttpRequest req,
            int id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Any }, async _ =>
            {
                Product product = await _productService.GetAsync(id);
                return ContentResult(product, ProductsContractResolver.Instance);
            });
        }

        [FunctionName("GetAllProducts")]
        [OpenApiOperation("GetAllProducts", "Products")]
        [OpenApiParameter(ListParameters.PageQueryStringKey, In = ParameterLocation.Query, Type = typeof(int), Description = "Page")]
        [OpenApiParameter(ListParameters.PageSizeQueryStringKey, In = ParameterLocation.Query, Type = typeof(short), Description = "Page size")]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(IList<Product>))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/products")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Any }, async _ =>
            {
                ListParameters lp = new (req);
                IList<Product> products = await _productService.GetAllAsync(lp.Page, lp.PageSize);
                return ContentResult (products, ProductsContractResolver.Instance);
            });
        }

        [FunctionName("AddProduct")]
        [OpenApiOperation("AddProduct", "Products", "Admin")]
        [OpenApiRequestBody(JsonContentType, typeof(Product))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Product))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/products")]
            HttpRequest req)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                Product input = await Serializer.DeserializeAsync<Product>(req.Body);
                Product product = await _productService.AddAsync(input);
                return ContentResult(product, ProductsContractResolver.Instance);
            });
        }

        [FunctionName("UpdateProduct")]
        [OpenApiOperation("UpdateProduct", "Products", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(int), Required = true)]
        [OpenApiRequestBody(JsonContentType, typeof(Product))]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithBody(HttpStatusCode.OK, JsonContentType, typeof(Product))]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "v1/products/{id:int}")]
            HttpRequest req,
            int id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                Product input = await Serializer.DeserializeAsync<Product>(req.Body);
                Product product = await _productService.UpdateAsync(id, input);
                return ContentResult(product, ProductsContractResolver.Instance);
            });
        }

        [FunctionName("RemoveProduct")]
        [OpenApiOperation("RemoveProduct", "Products", "Admin")]
        [OpenApiParameter("id", In = ParameterLocation.Path, Type = typeof(int), Required = true)]
        [OpenApiSecurity(IAuthService.BearerScheme, SecuritySchemeType.Http, BearerFormat = JwtBearerFormat, Scheme = OpenApiSecuritySchemeType.Bearer)]
        [OpenApiResponseWithoutBody(HttpStatusCode.NoContent)]
        [OpenApiResponseWithBody(HttpStatusCode.Unauthorized, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.Forbidden, PlainTextContentType, typeof(string))]
        [OpenApiResponseWithBody(HttpStatusCode.InternalServerError, PlainTextContentType, typeof(string))]
        public Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/products/{id:int}")]
            HttpRequest req,
            int id)
        {
            return ExecuteSafeSecurityValidatedAsync(req, new[] { Right.Admin }, async _ =>
            {
                await _productService.RemoveAsync(id);
                return new NoContentResult();
            });
        }
    }
}
