using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VideStore.Application.Interfaces;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.DTOs.Cart;
using VideStore.Shared.DTOs;
using VideStore.Api.Extensions;

namespace VideStore.Api.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        #region Get Cart
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCart([FromHeader] Guid? guestId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.GetCartAsync(userId, guestId);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }
        #endregion

        #region Add item to cart
        [HttpPost("add")]
        [ProducesResponseType(typeof(Result<CartDto>), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> AddItem(
            [FromBody] AddItemRequest request,
            [FromHeader] Guid? guestId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.AddItemAsync(userId, guestId, request);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();

        }
        #endregion

        #region Merge guest and user carts
        [HttpPost("merge")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MergeCarts([FromHeader] Guid guestId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _cartService.MergeCartsAsync(guestId, userId);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }
        #endregion

        #region Update item Quantity 
        [HttpPost("update")]
        [ProducesResponseType(typeof(Result), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        [ProducesResponseType(typeof(Result), 404)]
        public async Task<IActionResult> UpdateQuantity([FromHeader] Guid guestId, [FromBody] UpdateItemQuantityRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _cartService.UpdateItemQuantityAsync(userId, guestId, request);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
        }
        #endregion

        #region remove Specific item from cart
        [HttpDelete("remove-item/{productId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveItemFromCart([FromHeader] Guid guestId, [FromRoute] string productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _cartService.RemoveItemAsync(userId, guestId, productId);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }
        #endregion

        #region Clear Cart
        [HttpDelete("clear")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ClearCart([FromHeader] Guid guestId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _cartService.ClearCartAsync(userId, guestId);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();
        }
        #endregion

    }
}
