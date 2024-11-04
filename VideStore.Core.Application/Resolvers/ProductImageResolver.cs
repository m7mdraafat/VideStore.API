

using AutoMapper;
using VideStore.Application.Interfaces;
using VideStore.Application.Services;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Shared.Requests.Products;

namespace VideStore.Application.Resolvers
{
    public class ProductImageResolver(IImageService imageService) : IValueResolver<ProductRequest, Product, ICollection<ProductImage>>
    {
        public ICollection<ProductImage> Resolve(ProductRequest source, Product destination, ICollection<ProductImage> destMember, ResolutionContext context)
        {
            var productImages = new List<ProductImage>();
            if (source.ProductImages is { Count: > 0 })
            {
                foreach (var file in source.ProductImages)
                {
                    var folderType = "Products";
                    var id = destination.Id;

                    var imageUrl = imageService.SaveImageAsync(file, folderType, id).ToString();

                    productImages.Add(new ProductImage()
                    {
                        ImageUrl = imageUrl!, 
                        ProductId = destination.Id
                    });
                }
            }

            return productImages;
        }

   
    }
}
