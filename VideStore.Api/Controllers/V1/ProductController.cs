using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideStore.Api.Extensions;
using VideStore.Application.Interfaces;
using VideStore.Shared.DTOs.Requests.Products;
using VideStore.Shared.Specifications.ProductSpecifications;

namespace VideStore.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetProducts([FromQuery] ProductSpecifications specifications)
        {
            var result = await productService.GetAllProductsAsync(specifications);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetProducts(int id)
        {
            var result = await productService.GetProductAsync(id);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateProduct([FromForm]ProductRequest productRequest)
        {
            var result = await productService.CreateProductAsync(productRequest);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();

        }
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var result = await productService.DeleteProductAsync(id);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();

        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult> UpdateProduct(int id, [FromForm]ProductRequest productRequest)
        {
            var result = await productService.UpdateProductAsync(id, productRequest);
            
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();

        }
    }
}
