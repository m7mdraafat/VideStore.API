namespace VideStore.Shared.DTOs.Cart
{

    public record CartItemDto(
        string ProductId,                // ID of the product
        string ProductName,           // Name of the product
        string ProductImageUrl,       // URL of the product image
        decimal UnitPrice,            // Price per unit of the product
        int Quantity,                 // Quantity of the product in the cart
        decimal TotalPrice            // Total price for this item (UnitPrice * Quantity)
    );
}
