namespace VideStore.Shared.DTOs.Responses.ShoppingCart;

public class ShoppingCartResponse
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public List<CartItemResponse> Items { get; set; } = [];
    public decimal SubTotal { get; set; }
    public decimal ShippingPrice => (Items.Sum(x => x.Quantity) > 3) ? 0 : 35;
    public decimal Total => SubTotal + ShippingPrice;
}