using VideStore.Domain.ErrorHandling;
using VideStore.Shared.DTOs.Order;

namespace VideStore.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Result<OrderDto>> CreateOrderAsync(string? userId, CreateOrderRequest request);
        Task<Result<OrderDto>> GetOrderDetailsAsync(int orderId, string userId);
        Task<Result<List<OrderDto>>> GetUserOrdersAsync(string userId);
        Task<Result> CancelOrderAsync(int orderId, string userId);
    }
}
