using VideStore.Shared.DTOs.Responses.Products;

namespace VideStore.Shared.DTOs.Responses.ShoppingCart
{
    public class CartItemResponse
    {
        public string CartItemId { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public ProductResponse Product { get; set; } = null!;
    }
}
