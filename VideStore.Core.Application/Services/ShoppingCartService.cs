
using AutoMapper;
using VideStore.Application.Interfaces;
using VideStore.Domain.Entities.CartEntities;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Domain.Interfaces;
using VideStore.Domain.Specifications.ProductSpecifications;
using VideStore.Shared.DTOs.Requests.ShoppingCart;
using VideStore.Shared.DTOs.Responses.ShoppingCart;

namespace VideStore.Application.Services
{
    public class ShoppingCartService(IShoppingCartRepository cartRepository, IMapper mapper, IUnitOfWork unitOfWork) : IShoppingCartService
    {
        public async Task<Result<ShoppingCartResponse>> CreateOrUpdateShoppingCartAsync(ShoppingCartRequest cartDto)
        {

            var cart = mapper.Map<ShoppingCartRequest, ShoppingCart>(cartDto);
            
            //foreach(var item in cart.Items)
            //{
            //    var productSpecifications = new ProductWithAllRelatedDataSpecifications(item.ProductId);
            //    var  product = await unitOfWork.Repository<Product>().GetEntityAsync(productSpecifications);
            //    if(product == null)
            //    {
            //        return Result.Failure<ShoppingCartResponse>
            //            (new Error(400, $"The product with id {item.ProductId} incorrect. Please enter correct product id. "));
            //    }
            //    item.ProductImageCover = product!.ProductImages.FirstOrDefault()?.ImageUrl ?? string.Empty;
            //    item.ProductName = product.Name; 

                
            //}
            var createdOrUpdatedCart = await cartRepository.CreateOrUpdateShoppingCartAsync(cart);

            if (createdOrUpdatedCart is null)
                return Result.Failure<ShoppingCartResponse>(new Error(400,
                    "Failed to create or update the cart. Please try again."));

            var cartResponse = mapper.Map<ShoppingCart, ShoppingCartResponse>(createdOrUpdatedCart);
            return Result.Success(cartResponse);
        }

        public async Task<Result<ShoppingCartResponse>> GetShoppingCartAsync(string shoppingCartId)
        {
            var cart = await cartRepository.GetShoppingCartAsync(shoppingCartId) ?? new ShoppingCart(shoppingCartId);

            var cartResponse = mapper.Map<ShoppingCart, ShoppingCartResponse>(cart);

            return Result.Success(cartResponse);
        }

        public async Task DeleteShoppingCartAsync(string shoppingCartId)
        {
            await cartRepository.DeleteShoppingCartAsync(shoppingCartId); 
        }
    }
}
