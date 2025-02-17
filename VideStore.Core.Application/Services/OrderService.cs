using Microsoft.Extensions.Logging;
using VideStore.Application.Interfaces;
using VideStore.Domain.Entities.OrderEntities;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Domain.Interfaces;
using VideStore.Shared.DTOs.Cart;
using VideStore.Shared.DTOs.Order;
using VideStore.Shared.Specifications.OrderSpecifications;

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

        public async Task<Result<OrderDto>> GetOrderDetailsAsync(string orderId, string userId)
        {
            try
            {
                var orderSpec = new OrderWithItemsSpecifications(orderId);
                var order = await _unitOfWork.Repository<Order>().GetEntityAsync(orderSpec);


                if (order == null)
                    return Result.Failure<OrderDto>(new Error(404, "Order not found"));

                if (order.UserId != userId)
                    return Result.Failure<OrderDto>(new Error(403, "Unauthorized access to order"));

                return Result.Success(await MapToDto(order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order details");
                return Result.Failure<OrderDto>(new Error(500, "Error retrieving order details"));
            }
        }
        public async Task<Result<IReadOnlyCollection<OrderDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _unitOfWork.Repository<Order>().GetAllAsync();
                var orderDtos = new List<OrderDto>();

                foreach (var order in orders)
                {
                    orderDtos.Add(await MapToDto(order));
                }

                return Result.Success<IReadOnlyCollection<OrderDto>>(orderDtos.AsReadOnly());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all orders");
                return Result.Failure<IReadOnlyCollection<OrderDto>>(new Error(500, "Internal server error, please try again later."));
            }
        }
        public async Task<Result<List<OrderDto>>> GetUserOrdersAsync(string userId)
        {
            try
            {
                var orderSpec = new OrderWithItemsByUserSpecification(userId);
                var orders = await _unitOfWork.Repository<Order>().GetAllAsync(orderSpec);

                var orderDtos = new List<OrderDto>();
                foreach (var order in orders)
                {
                    orderDtos.Add(await MapToDto(order));
                }

                return Result.Success(orderDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user orders");
                return Result.Failure<List<OrderDto>>(new Error(500, "Error retrieving user orders"));
            }
        }

        public async Task<Result> CancelOrderAsync(string orderId, string userId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var orderSpecifications = new OrderWithItemsSpecifications(orderId);
                var order = await _unitOfWork.Repository<Order>().GetEntityAsync(orderSpecifications);

                if (order == null)
                    return Result.Failure(new Error(404, "Order not found"));

                if (order.UserId != userId)
                    return Result.Failure(new Error(403, "Unauthorized access to order"));

                if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
                    return Result.Failure(new Error(400, $"Order cannot be cancelled in current status: {order.Status.ToString()}"));

                // Update order status
                order.UpdateStatus(OrderStatus.Cancelled);
                _unitOfWork.Repository<Order>().Update(order);

                // Restore product stock
                await RestoreProductStock(order.Items);

                await _unitOfWork.CompleteAsync();

                await _unitOfWork.CommitTransactionAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error cancelling order");
                return Result.Failure(new Error(500, "Error cancelling order"));
            }
        }

        #region Helper Methods
        private async Task RestoreProductStock(IEnumerable<OrderItem> orderItems)
        {
            foreach (var item in orderItems)
            {
                var product = await _unitOfWork.Repository<Product>().GetEntityAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    _unitOfWork.Repository<Product>().Update(product);
                }
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


                if (product.Price > item.UnitPrice || product.Price < item.UnitPrice)
                    return Result.Failure(new Error(400, $"Price changed, Please try again."));
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
                order.Status.ToString(),
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

      
        #endregion

    }
}
