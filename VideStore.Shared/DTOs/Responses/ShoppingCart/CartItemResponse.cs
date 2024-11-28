using VideStore.Shared.DTOs.Responses.Products;

namespace VideStore.Shared.DTOs.Responses.ShoppingCart
{
    public class CartItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageCover { get; set; } = string.Empty;
        public string Sizes { get; set; } = null!;
        public string Color { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }
}
