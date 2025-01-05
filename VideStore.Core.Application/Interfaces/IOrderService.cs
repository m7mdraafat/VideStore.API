using System.Security.Claims;
using VideStore.Domain.Entities.OrderEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Shared.DTOs.Requests.Orders;
using VideStore.Shared.DTOs.Responses.Orders;

namespace VideStore.Application.Interfaces
{
    public interface IOrderService
    {
        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="orderRequest">The request object containing order details.</param>
        /// <returns>The created order as a response DTO.</returns>
        Task<Result<OrderResponse>> CreateOrderAsync(string cartId, OrderRequest orderRequest);

        /// <summary>
        /// Retrieves an order by its ID.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <returns>The order details as a response DTO, or null if not found.</returns>
        Task<Result<OrderResponse?>> GetOrderByIdAsync(int orderId);

        /// <summary>
        /// Retrieves all orders for a specific buyer.
        /// </summary>
        /// <param name="userClaims"></param>
        /// <returns>A list of orders for the specified buyer.</returns>
        Task<Result<IReadOnlyList<OrderResponse>>> GetOrdersForUserAsync();

        /// <summary>
        /// Retrieves all orders in the system.
        /// </summary>
        /// <returns>A list of all orders.</returns>
        Task<Result<IReadOnlyList<OrderResponse>>> GetAllOrdersAsync();

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="status">The new status of the order.</param>
        /// <returns>The updated order as a response DTO, or null if not found.</returns>
        Task<Result<OrderResponse?>> UpdateOrderStatusAsync(int orderId, OrderStatus status);

        /// <summary>
        /// Cancels an order by its ID.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <returns>True if the order was successfully canceled, otherwise false.</returns>
        Task<Result<string>> CancelOrderAsync(int orderId);


        /// <summary>
        /// Calculates the total cost of an order.
        /// </summary>
        /// <param name="subTotal">The subtotal of the order.</param>
        /// <param name="deliveryFee">The delivery fee.</param>
        /// <returns>The total cost of the order.</returns>
        decimal CalculateTotal(decimal subTotal, decimal deliveryFee);
    }
}
