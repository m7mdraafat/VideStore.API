using AutoMapper;
using VideStore.Application.Interfaces;
using VideStore.Application.Mapping.Resolvers;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Domain.Interfaces;
using VideStore.Shared.Requests.Categories;
using VideStore.Shared.Requests.Products;
using VideStore.Shared.Responses.Products;

namespace VideStore.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductRequest, Product>()
                .ForMember(dest => dest.ProductImages, opt => opt.Ignore())
                .ForMember(dest => dest.ProductSizes, opt => opt.MapFrom<ProductSizesResolver>());

            // Mapping for CategoryRequest to Category
            CreateMap<CategoryRequest, Category>()
                .ForMember(dest => dest.CoverImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore());

            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color != null ? src.Color.ColorName : null))
                .ForMember(dest => dest.ColorHexCode, opt => opt.MapFrom(src => src.Color != null ? src.Color.ColorHexCode : null))
                .ForMember(dest => dest.ProductImageUrls, opt => opt.MapFrom(src => src.ProductImages.Select(pi => pi.ImageUrl)))
                .ForMember(dest => dest.ProductSizes, opt => opt.MapFrom(src => src.ProductSizes));

            // Mapping for ProductSize to ProductSizeResponse
            CreateMap<ProductSize, ProductSizeResponse>()
                .ForMember(dest => dest.SizeName, opt => opt.MapFrom(src => src.Size != null ? src.Size.SizeName : null));
        }
    }
}