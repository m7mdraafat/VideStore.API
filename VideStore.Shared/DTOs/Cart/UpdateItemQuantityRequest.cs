namespace VideStore.Shared.DTOs.Cart
{
    // Application/DTOs/UpdateItemQuantityRequest.cs
    public record UpdateItemQuantityRequest(
        string ProductId,                // ID of the product to update
        int Quantity                  // New quantity of the product
    );
}
