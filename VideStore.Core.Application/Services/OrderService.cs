using Microsoft.Extensions.Logging;
using VideStore.Application.Interfaces;
using VideStore.Domain.Entities.OrderEntities;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Domain.Interfaces;
using VideStore.Shared.DTOs.Cart;
using VideStore.Shared.DTOs.Order;

namespace VideStore.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IUnitOfWork unitOfWork,
            ICartService cartService,
            ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
            _logger = logger;
        }

        public async Task<Result<OrderDto>> CreateOrderAsync(
            string? userId, CreateOrderRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Get cart items
                var cartResult = await _cartService.GetCartAsync(userId, null);
                if (!cartResult.IsSuccess || cartResult.Value?.Items.Count == 0)
                    return Result.Failure<OrderDto>(new Error(400, "Cart is empty"));

                // 2. Validate stock and prices
                var validationResult = await ValidateCartItems(cartResult.Value!.Items);
                if (!validationResult.IsSuccess)
                    return Result.Failure<OrderDto>(new Error(400, "Invalid cart items, Please try again."));

                // 4. Create order
                var orderItems = MapToOrderItems(cartResult.Value.Items);
                var order = new Order(
                    userId!,
                    request.ShippingAddress,
                    orderItems);

                await _unitOfWork.Repository<Order>().AddAsync(order);

                // 5. Update stock
                await UpdateProductStock(cartResult.Value.Items);

                // 6. Clear cart
                await _cartService.ClearCartAsync(userId, null);

                await _unitOfWork.CommitTransactionAsync();

                return Result.Success(await MapToDto(order));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Order creation failed");
                return Result.Failure<OrderDto>(new Error(500, "Order creation failed"));
            }
        }

        private async Task<Result> ValidateCartItems(List<CartItemDto> cartItems)
        {
            foreach (var item in cartItems)
            {
                var product = await _unitOfWork.Repository<Product>().GetEntityAsync(item.ProductId);
                if (product == null)
                    return Result.Failure(new Error(404, $"Product {item.ProductId} not found"));

                if (product.StockQuantity < item.Quantity)
                    return Result.Failure(new Error(400, $"Insufficient stock for {product.Name}"));
            }
            return Result.Success();
        }

        private List<OrderItem> MapToOrderItems(List<CartItemDto> cartItems)
        {
            return cartItems.Select(item => new OrderItem(
                item.ProductId,
                item.ProductName,
                item.UnitPrice,
                item.Quantity,
                item.ProductImageUrl
            )).ToList();
        }

        private async Task UpdateProductStock(List<CartItemDto> cartItems)
        {
            foreach (var item in cartItems)
            {
                var product = await _unitOfWork.Repository<Product>().GetEntityAsync(item.ProductId);
                product!.StockQuantity -= item.Quantity;
                _unitOfWork.Repository<Product>().Update(product);
            }
        }

        private async Task<OrderDto> MapToDto(Order order)
        {
            return new OrderDto(
                order.Id,
                order.OrderDate,
                order.Status,
                order.ShippingAddress,
                order.Items.Select(i => new OrderItemDto(
                    i.ProductId,
                    i.ProductName,
                    i.UnitPrice,
                    i.Quantity,
                    i.PictureUrl
                )).ToList(),
                order.Items.Sum(i => i.UnitPrice * i.Quantity)
            );
        }

        public Task<Result<OrderDto>> GetOrderDetailsAsync(int orderId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<OrderDto>>> GetUserOrdersAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result> CancelOrderAsync(int orderId, string userId)
        {
            throw new NotImplementedException();
        }

     
    }
}
