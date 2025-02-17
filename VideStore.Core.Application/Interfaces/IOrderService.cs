using VideStore.Domain.ErrorHandling;
using VideStore.Shared.DTOs.Order;

namespace VideStore.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Result<IReadOnlyCollection<OrderDto>>> GetAllOrders();
        Task<Result<OrderDto>> CreateOrderAsync(string? userId, CreateOrderRequest request);
        Task<Result<OrderDto>> GetOrderDetailsAsync(string orderId, string userId);
        Task<Result<List<OrderDto>>> GetUserOrdersAsync(string userId);
        Task<Result> CancelOrderAsync(string orderId, string userId);
    }
}
