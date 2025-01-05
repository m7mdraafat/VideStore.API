using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideStore.Api.Extensions;
using VideStore.Application.Interfaces;
using VideStore.Shared.DTOs.Requests.ShoppingCart;
using VideStore.Shared.DTOs.Responses.ShoppingCart;

namespace VideStore.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ShoppingCartController(IShoppingCartService shoppingCartService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<ShoppingCartResponse>> CreateOrUpdateCart(ShoppingCartRequest cart)
        {
            var result = await shoppingCartService.CreateOrUpdateShoppingCartAsync(cart);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [HttpGet]
        public async Task<ActionResult<ShoppingCartResponse>> GetShoppingCart(string shoppingCartId)
        {
            var result = await shoppingCartService.GetShoppingCartByIdAsync(shoppingCartId);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }

        [HttpDelete]
        public async Task DeleteShoppingCart(string shoppingCartId)
        {
            await shoppingCartService.DeleteShoppingCartAsync(shoppingCartId);
        }

    }
}
