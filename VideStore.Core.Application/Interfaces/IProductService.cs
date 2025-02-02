using VideStore.Application.DTOs;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Domain.Interfaces;
using VideStore.Shared.DTOs.Requests.Products;
using VideStore.Shared.DTOs.Responses.Products;
using VideStore.Shared.Specifications.ProductSpecifications;

namespace VideStore.Application.Interfaces
{
    public interface IProductService
    {
        Task<Result<PaginationToReturn<ProductResponse>>> GetAllProductsAsync(ProductSpecifications spec);
        Task<Result<ProductResponse>> GetProductAsync(string id);
        Task<Result<ProductResponse>> CreateProductAsync(ProductRequest productRequest);
        Task<Result<ProductResponse>> UpdateProductAsync(string id, ProductRequest productRequest);
        Task<Result<string>> DeleteProductAsync(string id);


    }
}
