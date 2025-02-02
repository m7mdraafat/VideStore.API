namespace VideStore.Shared.DTOs.Responses.ShoppingCart
{
    public class CartResponse
    {
        public string CartId { get; set; } = null!;
        public decimal Total => CartItems.Sum(p => p.Product.Price * p.Product.StockQuantity);
        public List<CartItemResponse> CartItems { get; set; } = new List<CartItemResponse>();

    }
}
