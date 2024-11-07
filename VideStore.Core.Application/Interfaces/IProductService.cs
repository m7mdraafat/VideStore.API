﻿using VideStore.Application.DTOs;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Domain.Interfaces;
using VideStore.Shared.Requests.Products;
using VideStore.Shared.Responses.Products;
using VideStore.Shared.Specifications.ProductSpecifications;

namespace VideStore.Application.Interfaces
{
    public interface IProductService
    {
        Task<Result<PaginationToReturn<ProductResponse>>> GetAllProductsAsync(ProductSpecifications spec);
        Task<Result<ProductResponse>> GetProductAsync(int id);
        Task<Result<ProductResponse>> CreateProductAsync(ProductRequest productRequest);
        Task<Result<ProductResponse>> UpdateProductAsync(int id, ProductRequest productRequest);
        Task<Result<string>> DeleteProductAsync(int id);


    }
}