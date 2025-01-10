using AutoMapper;
using VideStore.Application.Mapping.Resolvers;
using VideStore.Domain.Entities.IdentityEntities;
using VideStore.Domain.Entities.OrderEntities;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Shared.DTOs;
using VideStore.Shared.DTOs.Requests.Categories;
using VideStore.Shared.DTOs.Requests.Orders;
using VideStore.Shared.DTOs.Requests.Products;
using VideStore.Shared.DTOs.Responses.Orders;
using VideStore.Shared.DTOs.Responses.Products;
using UserAddressDto = VideStore.Shared.DTOs.Responses.Users.UserAddressDto;

namespace VideStore.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserAddressDto, UserAddress>()
                .ForMember(dest => dest.AppUser, opt => opt.Ignore())
                .ForMember(dest => dest.AppUserId, opt => opt.Ignore()).ReverseMap();

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

         

            // Mapping for OrderRequest to Order
            CreateMap<OrderRequest, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Mapping for Order to OrderResponse
            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.GetTotal()));

            // Mapping for OrderItemRequest to OrderItem
            CreateMap<OrderItemRequest, OrderItem>();

            // Mapping for OrderItem to OrderItemResponse
            CreateMap<OrderItem, OrderItemResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageCover, opt => opt.MapFrom(src => src.Product.ProductImages.FirstOrDefault()!.ImageUrl));
        }
    }
}