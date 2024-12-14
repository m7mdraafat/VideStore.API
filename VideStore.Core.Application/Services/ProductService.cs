using AutoMapper;
using Microsoft.AspNetCore.Http;
using VideStore.Application.DTOs;
using VideStore.Application.Interfaces;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Domain.Interfaces;
using VideStore.Shared.DTOs.Requests.Products;
using VideStore.Shared.DTOs.Responses.Products;
using VideStore.Shared.Specifications.ProductSpecifications;

namespace VideStore.Application.Services
{
    public class ProductService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService) : IProductService
    {
        public async Task<Result<PaginationToReturn<ProductResponse>>> GetAllProductsAsync(ProductSpecifications spec)
        {
            var specifications = new ProductWithAllRelatedDataSpecifications(spec);

            var products = await unitOfWork.Repository<Product>().GetAllAsync(specifications);

            var productResponse = mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductResponse>>(products);

            var productsCount = await _productCount(spec);
            if (productsCount > spec.PageSize)
                productsCount = spec.PageSize;

            var productsWithPagination =
                new PaginationToReturn<ProductResponse>(spec.PageIndex, spec.PageSize, productsCount, productResponse);

            return Result.Success<PaginationToReturn<ProductResponse>>(productsWithPagination); 
        }

        public async Task<Result<ProductResponse>> GetProductAsync(int id)
        {
            var specifications = new ProductWithAllRelatedDataSpecifications(id);

            var product = await unitOfWork.Repository<Product>().GetEntityAsync(specifications);

            if (product is null)
                return Result.Failure<ProductResponse>(new Error(404,
                    "The product you are looking for does not exists. Please Check the product ID and try again."));


            var productResponse = mapper.Map<ProductResponse>(product);

            return Result.Success<ProductResponse>(productResponse); 
        }

        public async Task<Result<ProductResponse>> CreateProductAsync(ProductRequest productRequest)
        {
            // Map the product without setting the images yet
            var product = mapper.Map<ProductRequest, Product>(productRequest);

            // Check if the category exists
            var category = await unitOfWork.Repository<Category>().GetEntityAsync(productRequest.CategoryId);
            if (category == null)
            {
                return Result.Failure<ProductResponse>(new Error(404,
                    "The category you are looking for does not exist. Please check the category ID and try again."));
            }

            product.Category = category;

            // Save the product to get the ID
            await unitOfWork.Repository<Product>().AddAsync(product);
            var result = await unitOfWork.CompleteAsync();

            if (result <= 0)
            {
                return Result.Failure<ProductResponse>(new Error(500,
                    "An error occurred while creating the product. Please try again."));
            }

            // Ensure product ID is set
            if (product.Id <= 0)
            {
                return Result.Failure<ProductResponse>(new Error(500,
                    "Product ID is not valid after saving. Check the database and repository logic."));
            }

            // Check if the color exists and set it
            var color = await unitOfWork.Repository<Color>().GetEntityAsync(productRequest.ColorId);
            if (color != null)
            {
                product.Color = color;
            }

            // Save product images asynchronously after getting the product ID
            if (productRequest.ProductImages is { Count: > 0 })
            {
                var productImages = new List<ProductImage>();
                foreach (var file in productRequest.ProductImages)
                {
                    // Save image to server and get the URL
                    var imageUrl = await imageService.SaveImageAsync(file, "Products", product.Id);

                    // Check if the image URL is valid
                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        return Result.Failure<ProductResponse>(new Error(500,
                            "An error occurred while saving one or more images. Image URL is invalid."));
                    }

                    // Create ProductImage object and add to the list
                    productImages.Add(new ProductImage
                    {
                        ImageUrl = imageUrl,
                        ProductId = product.Id
                    });
                }

                // Log before saving images
                Console.WriteLine($"Saving {productImages.Count} product images for Product ID: {product.Id}");
                await unitOfWork.Repository<ProductImage>().AddRangeAsync(productImages);
                Console.WriteLine("Product images added to the DbContext.");

                // Update the product with the new images
                product.ProductImages = productImages;
                unitOfWork.Repository<Product>().Update(product);

                // Save changes to the database
                var imageSaveResult = await unitOfWork.CompleteAsync();
                Console.WriteLine($"Image save result: {imageSaveResult}");

                if (imageSaveResult <= 0)
                {
                    return Result.Failure<ProductResponse>(new Error(500,
                        "An error occurred while saving product images. Please try again."));
                }
            }

            // Map the final product response
            var productResponse = mapper.Map<Product, ProductResponse>(product);

            return Result.Success(productResponse);
        }

        public async Task<Result<ProductResponse>> UpdateProductAsync(int id, ProductRequest productRequest)
        {
            // Retrieve the existing product with related data
            var spec = new ProductWithAllRelatedDataSpecifications(id);
            var existingProduct = await unitOfWork.Repository<Product>().GetEntityAsync(spec);
            if (existingProduct is null)
            {
                return Result.Failure<ProductResponse>(new Error(404, "The product you are looking for does not exist."));
            }

            // Map updated properties from the request to the existing product
            mapper.Map(productRequest, existingProduct);

            // Handle category update
            var category = await unitOfWork.Repository<Category>().GetEntityAsync(productRequest.CategoryId);
            if (category is null) return Result.Failure<ProductResponse>(new Error(404, "Category not found."));
            existingProduct.Category = category;

            // Handle color update
            var color = await unitOfWork.Repository<Color>().GetEntityAsync(productRequest.ColorId);
            if (color != null) existingProduct.Color = color;

            //// Retrieve current product sizes
            //var existingProductSizes = existingProduct.ProductSizes.ToList();

            //// Get new and removed sizes
            //var newSizeIds = productRequest.SizeIds.Except(existingProductSizes.Select(ps => ps.SizeId)).ToList();
            //var removedSizeIds = existingProductSizes.Select(ps => ps.SizeId).Except(productRequest.SizeIds).ToList();

            //// Efficiently remove sizes that are no longer needed
            //if (removedSizeIds.Any())
            //{
            //    var sizesToRemove = existingProductSizes.Where(ps => removedSizeIds.Contains(ps.SizeId)).ToList();
            //    foreach (var sizeToRemove in sizesToRemove)
            //    {
            //        existingProduct.ProductSizes.Remove(sizeToRemove);
            //        unitOfWork.Repository<ProductSize>().Delete(sizeToRemove); // Remove ProductSize from DB
            //    }
            //}

            //// Efficiently add new sizes
            //if (newSizeIds.Any())
            //{

            //    foreach (var sizeId in newSizeIds)
            //    {
            //        var productSize = new ProductSize
            //        {
            //            SizeId = sizeId,
            //            ProductId = existingProduct.Id
            //        };
            //        existingProduct.ProductSizes.Add(productSize);
            //        await unitOfWork.Repository<ProductSize>().AddAsync(productSize); 
            //    }
            //}

            foreach (var productSize in existingProduct.ProductSizes )
            {
                productSize.ProductId = existingProduct.Id;
            }

            // Handle image addition and removal
            await HandleProductImagesAsync(existingProduct, productRequest.ProductImages);

            // Update the product in the repository
            unitOfWork.Repository<Product>().Update(existingProduct);

            // Save changes to the database
            var result = await unitOfWork.CompleteAsync();
            if (result <= 0)
            {
                return Result.Failure<ProductResponse>(new Error(500, "An error occurred while updating the product."));
            }

            // Map the updated product to a response DTO
            var productResponse = mapper.Map<Product, ProductResponse>(existingProduct);

            return Result.Success(productResponse);
        }

        public async Task<Result<string>> DeleteProductAsync(int id)
        {
            var product = await unitOfWork.Repository<Product>().GetEntityAsync(id);

            if(product is null)
                return Result.Failure<string>(new Error(404,
                    "The product you are looking for does not exists. Please Check the product ID and try again."));

            var imagesDeleted = await imageService.DeleteFolderAsync($"Images/Products/Product-{id}");

            unitOfWork.Repository<Product>().Delete(product);
            var result = await unitOfWork.CompleteAsync();
            return result <= 0
                ? Result.Failure<string>(new Error(500, "An error occured while deleting a product."))
                : Result.Success<string>("Product deleted successfully.");

        }



        private async Task<int> _productCount(ProductSpecifications specifications)
        {
            var spec = new ProductCountSpecifications(specifications);

            var productCount = await unitOfWork.Repository<Product>().GetCountAsync(spec);

            return productCount;
        }

        private async Task HandleProductImagesAsync(Product existingProduct, List<IFormFile> productImages)
        {
            if (productImages is { Count: > 0 })
            {
                var currentImageUrls = existingProduct.ProductImages.Select(pi => pi.ImageUrl).ToList();
                var imagesToRemove = currentImageUrls.Where(url => productImages.All(file => file.FileName != url)).ToList();

                foreach (var imageUrl in imagesToRemove)
                {
                    await imageService.DeleteImageAsync(imageUrl);

                    var productImageToRemove = existingProduct.ProductImages
                        .FirstOrDefault(pi => pi.ImageUrl == imageUrl);
                    if (productImageToRemove != null)
                    {
                        existingProduct.ProductImages.Remove(productImageToRemove);
                    }
                }

                foreach (var file in productImages)
                {
                    var imageUrl = await imageService.SaveImageAsync(file, "Products", existingProduct.Id);
                    var productImage = new ProductImage
                    {
                        ImageUrl = imageUrl,
                        ProductId = existingProduct.Id
                    };
                    existingProduct.ProductImages.Add(productImage);
                }
            }
        }
    }
}
