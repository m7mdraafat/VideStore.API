

using VideStore.Shared.DTOs.Cart;

namespace VideStore.Shared.DTOs
{
    public record CartDto(
    Guid? GuestId,                // Guest ID (if the cart belongs to a guest)
    string? UserId,               // User ID (if the cart belongs to an authenticated user)
    List<CartItemDto> Items,      // List of items in the cart
    decimal TotalPrice            // Total price of all items in the cart
);
}
