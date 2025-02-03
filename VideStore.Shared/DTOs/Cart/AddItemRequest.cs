
namespace VideStore.Shared.DTOs.Cart
{
    public record AddItemRequest(
        string ProductId,                // ID of the product to add
        int Quantity                  // Quantity of the product to add
    );
}
