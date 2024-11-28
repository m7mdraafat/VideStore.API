namespace VideStore.Shared.DTOs.Requests.ShoppingCart
{
    public class CartItemRequest
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageCover { get; set; } = null!;
        public string Sizes { get; set; } = null!;
        public string Color { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    
}
