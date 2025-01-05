using VideStore.Domain.Common;
using VideStore.Domain.Entities.ProductEntities;

namespace VideStore.Domain.Entities.CartEntities;

public class CartItem
{
    public CartItem() { }

    public CartItem(int productId, string productName, string productImageCover, int quantity, decimal unitPrice)
    {

        ProductId = productId;
        ProductName = productName;
        ProductImageCover = productImageCover;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductImageCover { get; set; } = null!;
    public string Sizes { get; set; } = null!;
    public string Color { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total => GetTotal();

    private decimal GetTotal() => Quantity * UnitPrice;
}