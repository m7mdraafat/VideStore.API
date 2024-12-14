
using AutoMapper;
using System.Security.Claims;
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
    public class ShoppingCartService(IShoppingCartRepository cartRepository, IMapper mapper) : IShoppingCartService
    {
        public async Task<Result<ShoppingCartResponse>> CreateOrUpdateShoppingCartAsync(ShoppingCartRequest cartDto)
        {
            if (cartDto.Id == null)
                return Result.Failure<ShoppingCartResponse>(new Error(400, "Cart id cannot be null."));

            var cart = mapper.Map<ShoppingCartRequest, ShoppingCart>(cartDto);
            
            var createdOrUpdatedCart = await cartRepository.CreateOrUpdateShoppingCartAsync(cart);

            if (createdOrUpdatedCart is null)
                return Result.Failure<ShoppingCartResponse>(new Error(400,
                    "Failed to create or update the cart. Please try again."));

            var cartResponse = mapper.Map<ShoppingCart, ShoppingCartResponse>(createdOrUpdatedCart);
            return Result.Success(cartResponse);
        }

        public async Task<Result<ShoppingCartResponse>> GetShoppingCartAsync(string shoppingCartId)
        {
            var cart = await cartRepository.GetShoppingCartAsync(shoppingCartId) ?? new ShoppingCart();

            var cartResponse = mapper.Map<ShoppingCart, ShoppingCartResponse>(cart);

            return Result.Success(cartResponse);
        }

        public async Task DeleteShoppingCartAsync(string shoppingCartId)
        {
            await cartRepository.DeleteShoppingCartAsync(shoppingCartId); 
        }

       
    }
}
