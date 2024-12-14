using Microsoft.AspNetCore.Http;
using VideStore.Application.Interfaces;
using VideStore.Domain.Entities.OrderEntities;
using VideStore.Domain.Interfaces;
using VideStore.Shared.DTOs.Requests.Orders;
using VideStore.Shared.DTOs.Responses.Orders;

namespace VideStore.Application.Services
{
    public class OrderService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : IOrderService
    {
        public async Task<OrderResponse> CreateOrderAsync(OrderRequest orderRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<OrderResponse>> GetOrdersByBuyerAsync(string buyerEmail)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<OrderResponse>> GetAllOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<OrderResponse?> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public decimal CalculateDeliveryFee(string governorate)
        {
            throw new NotImplementedException();
        }

        public decimal CalculateTotal(decimal subTotal, decimal deliveryFee)
        {
            throw new NotImplementedException();
        }
    }
}
