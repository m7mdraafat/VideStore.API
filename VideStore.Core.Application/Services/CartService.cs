using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VideStore.Application.Interfaces;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Domain.ErrorHandling;
using VideStore.Domain.Interfaces;
using VideStore.Domain.Specifications.ProductSpecifications;
using VideStore.Shared.DTOs;
using VideStore.Shared.DTOs.Cart;
using VideStore.Shared.Specifications;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CartService> _logger;

    public CartService(
        IUnitOfWork unitOfWork,
        IDistributedCache cache,
        ILogger<CartService> logger)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<CartDto>> GetCartAsync(string? userId, Guid? guestId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId) && !guestId.HasValue)
            {
                return Result.Failure<CartDto>(new Error(400, "Either user ID or guest ID must be provided"));
            }

            var cart = await GetOrCreateCartAsync(userId, guestId);
            if (cart == null)
            {
                _logger.LogWarning("Cart not found for user {UserId} or guest {GuestId}", userId, guestId);
                return Result.Failure<CartDto>(new Error(404, "Cart not found"));
            }

            var cartDto = await MapToDto(cart);
            _logger.LogInformation("Returning cart with {ItemCount} items", cartDto.Items.Count);
            return Result.Success(cartDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get cart");
            return Result.Failure<CartDto>(new Error(500, "Failed to retrieve cart"));
        }
    }
    public async Task<Result<CartDto>> AddItemAsync(string? userId, Guid? guestId, AddItemRequest request)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (string.IsNullOrEmpty(userId) && !guestId.HasValue)
            {
                return Result.Failure<CartDto>(new Error(400, "Either user ID or guest ID must be provided"));
            }
            var productResult = await ValidateProduct(request.ProductId, request.Quantity);
            if (!productResult.IsSuccess) return Result.Failure<CartDto>(productResult.Error);

            var cart = await GetOrCreateCartAsync(userId, guestId);
            cart.AddItem(request.ProductId, request.Quantity);

            _unitOfWork.Repository<Cart>().Update(cart);
            await _unitOfWork.CompleteAsync();

            // Update cache
            var cacheKey = GetCacheKey(userId, guestId);
            await CacheCartAsync(cacheKey, cart);

            await _unitOfWork.CommitTransactionAsync();

            var cartDto = await MapToDto(cart);
            return Result<CartDto>.Success(cartDto);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Failed to add item to cart");
            return Result.Failure<CartDto>(new Error(500, "Failed to add item to cart"));
        }
    }
    public async Task<Result<CartDto>> UpdateItemQuantityAsync(
    string? userId, Guid? guestId, UpdateItemQuantityRequest request)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) && !guestId.HasValue)
            {
                return Result.Failure<CartDto>(new Error(400, "Either user ID or guest ID must be provided"));
            }
            var cart = await GetOrCreateCartAsync(userId, guestId);
            if (cart == null)
            {
                return Result.Failure<CartDto>(new Error(404, "Cart not found"));
            }

            // Check if item exists in cart
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (existingItem == null)
            {
                return Result.Failure<CartDto>(new Error(404, "Item not found in cart"));
            }

            // Validate the NEW quantity 
            var productResult = await ValidateProduct(request.ProductId, request.Quantity);
            if (!productResult.IsSuccess)
            {
                return Result.Failure<CartDto>(productResult.Error);
            }

            // Set the new quantity
            existingItem.UpdateQuantity(request.Quantity);

            _unitOfWork.Repository<Cart>().Update(cart);
            await _unitOfWork.CompleteAsync();

            // Update cache
            var cacheKey = GetCacheKey(userId, guestId);
            await CacheCartAsync(cacheKey, cart);

            await _unitOfWork.CommitTransactionAsync();

            var cartDto = await MapToDto(cart);
            return Result.Success(cartDto);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Failed to update product quantity in cart");
            return Result.Failure<CartDto>(new Error(500, "Failed to update quantity"));
        }
    }
    public async Task<Result> RemoveItemAsync(string? userId, Guid? guestId, string productId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) && !guestId.HasValue)
            {
                return Result.Failure(new Error(400, "Either user ID or guest ID must be provided"));
            }

            // Build cart specification
            var cartSpec = !string.IsNullOrEmpty(userId)
                ? new CartSpecification(userId)
                : new CartSpecification(guestId.Value);

            // Retrieve existing cart
            var cart = await _unitOfWork.Repository<Cart>().GetEntityAsync(cartSpec);
            if (cart == null)
            {
                return Result.Failure(new Error(404, "Cart not found"));
            }

            // Check if item exists in cart
            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem == null)
            {
                return Result.Failure(new Error(404, "Item not found in cart"));
            }

            // Remove item from cart
            cart.Remove(cartItem);

            // Update cart in repository
            if (cart.Items.Count == 0)
            {
                _unitOfWork.Repository<Cart>().Delete(cart);
            }
            else
            {
                _unitOfWork.Repository<Cart>().Update(cart);
            }

            await _unitOfWork.CompleteAsync();

            // Update cache
            var cacheKey = GetCacheKey(userId, guestId);
            await InvalidateCacheAsync(cacheKey);

            await _unitOfWork.CommitTransactionAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Failed to remove item from cart");
            return Result.Failure(new Error(500, "Failed to remove item from cart"));
        }
    }
    public async Task<Result> MergeCartsAsync(Guid guestId, string userId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {

            // Get the guest cart
            var guestCartSpec = new CartSpecification(guestId);
            var guestCart = await _unitOfWork.Repository<Cart>().GetEntityAsync(guestCartSpec);

            if (guestCart == null)
            {
                return Result.Failure(new Error(404, "Guest cart not found"));
            }

            // Get or create the user cart
            var userCartSpec = new CartSpecification(userId);
            var userCart = await _unitOfWork.Repository<Cart>().GetEntityAsync(userCartSpec)
                ?? new Cart(userId);

            // Merge items from guest cart into user cart
            foreach (var guestItem in guestCart.Items)
            {
                var existingItem = userCart.Items.FirstOrDefault(i => i.ProductId == guestItem.ProductId);
                if (existingItem != null)
                {
                    // If the item already exists in the user cart, update the quantity
                    existingItem.UpdateQuantity(existingItem.Quantity + guestItem.Quantity);
                }
                else
                {
                    // If the item doesn't exist, add it to the user cart
                    userCart.AddItem(guestItem.ProductId, guestItem.Quantity);
                }
            }

            // Save the updated user cart

            if (userCart.Items.Count == 0)
            {
                await _unitOfWork.Repository<Cart>().AddAsync(userCart);
            }
            else
            {
                _unitOfWork.Repository<Cart>().Update(userCart);
            }

            // Delete the guest cart
            _unitOfWork.Repository<Cart>().Delete(guestCart);

            // Commit the transaction
            await _unitOfWork.CommitTransactionAsync();
            await _unitOfWork.CompleteAsync();

            // Update the cache
            var userCacheKey = GetCacheKey(userId, null);
            await CacheCartAsync(userCacheKey, userCart);

            var guestCacheKey = GetCacheKey(null, guestId);
            await InvalidateCacheAsync(guestCacheKey);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Failed to merge carts");
            return Result.Failure(new Error(500, "Failed to merge carts"));
        }
    }
    public async Task<Result> ClearCartAsync(string? userId, Guid? guestId)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) && !guestId.HasValue)
            {
                return Result.Failure(new Error(400, "Either user ID or guest ID must be provided"));
            }

            // Build cart specification
            var cartSpec = !string.IsNullOrEmpty(userId)
                ? new CartSpecification(userId)
                : new CartSpecification(guestId.Value);

            // Retrieve existing cart
            var cart = await _unitOfWork.Repository<Cart>().GetEntityAsync(cartSpec);
            if (cart == null)
            {
                return Result.Failure(new Error(404, "Cart not found"));
            }

            // Clear all items from the cart
            cart.Clear();

            // Update database
            if (cart.Items.Count == 0)
            {
                _unitOfWork.Repository<Cart>().Delete(cart);
            }

            await _unitOfWork.CompleteAsync();

            // Invalidate cache
            var cacheKey = GetCacheKey(userId, guestId);
            await InvalidateCacheAsync(cacheKey);

            await _unitOfWork.CommitTransactionAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Failed to clear cart");
            return Result.Failure(new Error(500, "Failed to clear cart"));
        }
    }

    #region Cache helpers
    private async Task<Cart> GetOrCreateCartAsync(string? userId, Guid? guestId)
    {
        var cacheKey = GetCacheKey(userId, guestId);
        var cachedCart = await GetCachedCart(cacheKey);
        if (cachedCart != null) return cachedCart;

        var cart_spec = new CartSpecification();
        if (!string.IsNullOrEmpty(userId))
            cart_spec = new CartSpecification(userId);
        else if (guestId.HasValue)
            cart_spec = new CartSpecification(guestId.Value);

        var cart = userId != null
            ? await _unitOfWork.Repository<Cart>().GetEntityAsync(cart_spec)
            : await _unitOfWork.Repository<Cart>().GetEntityAsync(cart_spec);

        if (cart == null)
        {
            cart = userId != null ? new Cart(userId) : new Cart(guestId!.Value);
            await _unitOfWork.Repository<Cart>().AddAsync(cart);
            await _unitOfWork.CompleteAsync();
        }

        await CacheCartAsync(cacheKey, cart);
        return cart;
    }
    private async Task<Cart?> GetCachedCart(string cacheKey)
    {
        try
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (cachedData == null) return null;

            return JsonSerializer.Deserialize<Cart>(cachedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache read failed");
            return null;
        }
    }
    private async Task CacheCartAsync(string cacheKey, Cart cart)
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = cart.UserId != null
                    ? TimeSpan.FromMinutes(15)
                    : TimeSpan.FromDays(3)
            };

            var serializedCart = JsonSerializer.Serialize(cart);
            await _cache.SetStringAsync(cacheKey, serializedCart, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache write failed");
        }
    }
    private async Task InvalidateCacheAsync(string cacheKey)
    {
        try
        {
            await _cache.RemoveAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache invalidation failed");
        }
    }
    private string GetCacheKey(string? userId, Guid? guestId)
        => userId != null ? $"VideStore_user_cart_{userId}" : $"VideStore_guest_cart_{guestId}";
    #endregion
    private async Task<Result> ValidateProduct(string productId, int quantity)
    {

        var product = await _unitOfWork.Repository<Product>().GetEntityAsync(productId);

        if (product == null) return Result.Failure(new Error(404, "Invalid quantity"));
        if (quantity <= 0) return Result.Failure(new Error(400, "Invalid quantity"));
        if (product.StockQuantity < quantity) return Result.Failure(new Error(400, "Invalid quantity"));

        return Result.Success();
    }
    private async Task<CartDto> MapToDto(Cart cart)
    {
        var items = new List<CartItemDto>();
        foreach (var item in cart.Items)
        {
            var product_spec = new ProductWithAllRelatedDataSpecifications(item.ProductId);
            var product = await _unitOfWork.Repository<Product>().GetEntityAsync(product_spec);
            items.Add(new CartItemDto(
                item.ProductId,
                product?.Name ?? "Unknown Product",
                product?.ProductImages.FirstOrDefault()!.ImageUrl ?? string.Empty,
                product?.Price ?? 0,
                item.Quantity,
                (product?.Price ?? 0) * item.Quantity
            ));
        }

        var dto = new CartDto(
            cart.GuestId,
            cart.UserId,
            items,
            items.Sum(i => i.TotalPrice)
        );

        return dto;
    }

}