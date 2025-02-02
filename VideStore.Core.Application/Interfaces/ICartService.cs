using VideStore.Domain.ErrorHandling;
using VideStore.Shared.DTOs;
using VideStore.Shared.DTOs.Cart;

namespace VideStore.Application.Interfaces
{
    public interface ICartService
    {
        Task<Result<CartDto>> GetCartAsync(string? userId, Guid? guestId);
        Task<Result<CartDto>> AddItemAsync(string? userId, Guid? guestId, AddItemRequest request);
        Task<Result<CartDto>> UpdateItemQuantityAsync(string? userId, Guid? guestId, UpdateItemQuantityRequest request);
        Task<Result> RemoveItemAsync(string? userId, Guid? guestId, string productId);
        Task<Result> MergeCartsAsync(Guid guestId, string userId);
        Task<Result> ClearCartAsync(string? userId, Guid? guestId);
    }
}
