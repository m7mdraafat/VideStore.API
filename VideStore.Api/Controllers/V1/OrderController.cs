using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideStore.Application.Interfaces;
using VideStore.Shared.DTOs.Order;
using VideStore.Domain.ErrorHandling;
using System.Security.Claims;
using VideStore.Api.Extensions;

namespace VideStore.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;

        /// <summary>
        /// Creates a new order from the current user's cart
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _orderService.CreateOrderAsync(userId, request);

            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();
            
        }

        /// <summary>
        /// Gets details for a specific order
        /// </summary>
        [HttpGet("{orderId}")]
        [Authorize]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetOrderDetails(string orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.GetOrderDetailsAsync(orderId, userId);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();

        }

        /// <summary>
        /// Gets all orders for the current user
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.GetUserOrdersAsync(userId);
            return result.IsSuccess ? result.ToSuccess(result.Value) : result.ToProblem();

        }

        /// <summary>
        /// Cancels a specific order
        /// </summary>
        [HttpPut("{orderId}/cancel")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> CancelOrder(string orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.CancelOrderAsync(orderId, userId);
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();

        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrders();
            return result.IsSuccess ? result.ToSuccess() : result.ToProblem();

        }

    }
}