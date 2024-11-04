using AutoMapper;
using Microsoft.AspNetCore.Http;
using VideStore.Application.Interfaces;
using VideStore.Application.Resolvers;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Shared.Requests.Categories;
using VideStore.Shared.Requests.Products;
using VideStore.Shared.Responses.Categories;
using VideStore.Shared.Responses.Products;

namespace VideStore.Application.Mapping
{
    public class MappingProfile : Profile
    {
        private readonly IImageService _imageService;
        public MappingProfile(IImageService imageService)
        {
            _imageService = imageService;

            CreateMap<ProductRequest, Product>()
                .ForMember(dest => dest.ProductImages, opt => opt.MapFrom<ProductImageResolver>())
                .ForMember(dest => dest.ProductSizes, opt => opt.MapFrom(src => MapProductSizes(src.SizeIds)));


            // Mapping for CategoryRequest to Category
            CreateMap<CategoryRequest, Category>()
                .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.CoverImageUrl))
                .ForMember(dest => dest.Products, opt => opt.Ignore());

            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.ProductColor.ColorName))
                .ForMember(dest => dest.ColorHexCode, opt => opt.MapFrom(src => src.ProductColor.ColorHexCode))
                .ForMember(dest => dest.ProductImageUrls, opt => opt.MapFrom(src => src.ProductImages.Select(pi => pi.ImageUrl)))
                .ForMember(dest => dest.ProductSizes, opt => opt.MapFrom(src => src.ProductSizes));

            // Mapping for ProductSize to ProductSizeResponse
            CreateMap<ProductSize, ProductSizeResponse>();
        }

        private static List<ProductSize> MapProductSizes(List<int> sizeIds)
        {
            return sizeIds.Select(sizeId => new ProductSize
            {
                SizeId = sizeId
            }).ToList();
        }
    }
}
