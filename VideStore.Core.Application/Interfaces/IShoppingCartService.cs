using System.Security.Claims;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.DTOs.Requests.ShoppingCart;
using VideStore.Shared.DTOs.Responses.ShoppingCart;

namespace VideStore.Application.Interfaces
{
    public interface IShoppingCartService
    {
        Task<Result<ShoppingCartResponse>> CreateOrUpdateShoppingCartAsync(ShoppingCartRequest cartDto);
        Task<Result<ShoppingCartResponse>> GetShoppingCartByIdAsync(string shoppingCartId);
        Task DeleteShoppingCartAsync(string shoppingCartId); 
    }
}
