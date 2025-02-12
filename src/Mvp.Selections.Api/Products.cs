using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mvp.Selections.Api.Model.Request;
using Mvp.Selections.Api.Serialization.ContractResolvers;
using Mvp.Selections.Api.Serialization.Interfaces;
using Mvp.Selections.Api.Services.Interfaces;
using Mvp.Selections.Domain;

namespace Mvp.Selections.Api;

public class Products(
    ILogger<Products> logger,
    ISerializer serializer,
    IAuthService authService,
    IProductService productService)
    : Base<Products>(logger, serializer, authService)
{
    [Function("GetProduct")]
    public Task<IActionResult> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/products/{id:int}")]
        HttpRequest req,
        int id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Any], async _ =>
        {
            Product? product = await productService.GetAsync(id);
            return ContentResult(product, ProductsContractResolver.Instance);
        });
    }

    [Function("GetAllProducts")]
    public Task<IActionResult> GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, GetMethod, Route = "v1/products")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Any], async _ =>
        {
            ListParameters lp = new(req);
            IList<Product> products = await productService.GetAllAsync(lp.Page, lp.PageSize);
            return ContentResult(products, ProductsContractResolver.Instance);
        });
    }

    [Function("AddProduct")]
    public Task<IActionResult> Add(
        [HttpTrigger(AuthorizationLevel.Anonymous, PostMethod, Route = "v1/products")]
        HttpRequest req)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            Product? input = await Serializer.DeserializeAsync<Product>(req.Body);
            Product? product = input != null ? await productService.AddAsync(input) : null;
            return ContentResult(product, ProductsContractResolver.Instance);
        });
    }

    [Function("UpdateProduct")]
    public Task<IActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Anonymous, PatchMethod, Route = "v1/products/{id:int}")]
        HttpRequest req,
        int id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            Product? input = await Serializer.DeserializeAsync<Product>(req.Body);
            Product? product = input != null ? await productService.UpdateAsync(id, input) : null;
            return ContentResult(product, ProductsContractResolver.Instance);
        });
    }

    [Function("RemoveProduct")]
    public Task<IActionResult> Remove(
        [HttpTrigger(AuthorizationLevel.Anonymous, DeleteMethod, Route = "v1/products/{id:int}")]
        HttpRequest req,
        int id)
    {
        return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ =>
        {
            await productService.RemoveAsync(id);
            return new NoContentResult();
        });
    }
}