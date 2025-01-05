using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using VideStore.Application.Interfaces;
using VideStore.Domain.Entities.OrderEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Domain.Interfaces;
using VideStore.Shared.DTOs.Requests.Orders;
using VideStore.Shared.DTOs.Responses.Orders;
using VideStore.Shared.Specifications.OrderSpecifications;

namespace VideStore.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public OrderService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }

        public async Task<Result<OrderResponse>> CreateOrderAsync(string cartId, OrderRequest orderRequest)
        {
            var userEmail = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return Result.Failure<OrderResponse>(new Error(401, "User is not authenticated"));
            }

            var order = new Order
            {
                BuyerEmail = userEmail,
                OrderDate = DateTimeOffset.UtcNow,
                ShippingAddress = mapper.Map<OrderAddress>(orderRequest.ShippingAddress),
                OrderItems = mapper.Map<ICollection<OrderItem>>(orderRequest.OrderItems),
                SubTotal = orderRequest.SubTotal,
                Status = OrderStatus.Pending
            };

            await unitOfWork.Repository<Order>().AddAsync(order);
            await unitOfWork.CompleteAsync();

            var orderResponse = mapper.Map<OrderResponse>(order);
            return Result<OrderResponse>.Success(orderResponse);
        }

        public async Task<Result<OrderResponse?>> GetOrderByIdAsync(int orderId)
        {
            var spec = new OrderWithItemsSpecifications(orderId);
            var order = await unitOfWork.Repository<Order>().GetEntityAsync(spec);

            if (order == null)
            {
                return Result.Failure<OrderResponse?>(new Error(404, "Order not found"));
            }

            var orderResponse = mapper.Map<OrderResponse>(order);
            return Result<OrderResponse?>.Success(orderResponse)!;
        }

        public async Task<Result<IReadOnlyList<OrderResponse>>> GetOrdersForUserAsync()
        {
            var userEmail = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return Result.Failure<IReadOnlyList<OrderResponse>>(new Error(404, "User is not authenticated"));
            }

            var spec = new OrderWithItemsByUserSpecification(userEmail);
            var orders = await unitOfWork.Repository<Order>().GetAllAsync(spec);

            var orderResponses = mapper.Map<IReadOnlyList<OrderResponse>>(orders);
            return Result.Success(orderResponses);
        }

        public async Task<Result<IReadOnlyList<OrderResponse>>> GetAllOrdersAsync()
        {
            var spec = new OrderWithItemsSpecifications();
            var orders = await unitOfWork.Repository<Order>().GetAllAsync(spec);

            var orderResponses = mapper.Map<IReadOnlyList<OrderResponse>>(orders);
            return Result.Success(orderResponses);
        }

        public async Task<Result<OrderResponse?>> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await unitOfWork.Repository<Order>().GetEntityAsync(orderId);
            if (order == null)
            {
                return Result.Failure<OrderResponse?>(new Error(404, "Order not found"));
            }

            order.Status = status;
            unitOfWork.Repository<Order>().Update(order);
            await unitOfWork.CompleteAsync();

            var orderResponse = mapper.Map<OrderResponse>(order);
            return Result.Success(orderResponse)!;
        }

        public async Task<Result<string>> CancelOrderAsync(int orderId)
        {
            var order = await unitOfWork.Repository<Order>().GetEntityAsync(orderId);
            if (order == null)
            {
                return Result.Failure<string>(new Error(404, "Order not found"));
            }

            order.Status = OrderStatus.Cancelled;
            unitOfWork.Repository<Order>().Update(order);
            await unitOfWork.CompleteAsync();

            return Result.Success<string>("Order cancelled successfully");
        }

        public decimal CalculateTotal(decimal subTotal, decimal deliveryFee)
        {
            return subTotal + deliveryFee;
        }
    }
}
