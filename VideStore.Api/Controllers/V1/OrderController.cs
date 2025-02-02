using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideStore.Application.Interfaces;
using VideStore.Domain.Entities.OrderEntities;
using VideStore.Shared.DTOs.Requests.Orders;

namespace VideStore.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(string cartId, [FromBody] OrderRequest orderRequest)
        {
            var result = await _orderService.CreateOrderAsync(cartId, orderRequest);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetOrderById), new { orderId = result.Value.OrderId }, result.Value);
            }
            return BadRequest(result.Error);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            var result = await _orderService.GetOrderByIdAsync(orderId);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return NotFound(result.Error);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetOrdersForUser()
        {
            var result = await _orderService.GetOrdersForUserAsync();
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return BadRequest(result.Error);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return BadRequest(result.Error);
        }

        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] OrderStatus status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, status);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return NotFound(result.Error);
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> CancelOrder(string orderId)
        {
            var result = await _orderService.CancelOrderAsync(orderId);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return NotFound(result.Error);
        }
    }
}
