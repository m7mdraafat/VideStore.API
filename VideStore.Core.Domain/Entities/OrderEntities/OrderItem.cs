using VideStore.Domain.Common;
using VideStore.Domain.Entities.ProductEntities;

namespace VideStore.Domain.Entities.OrderEntities;

public class OrderItem : BaseEntity
{
    public OrderItem() { }

    public OrderItem(string orderId, string productId, string productName, string productImageCover ,int quantity, decimal unitPrice)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        ProductImageCover = productImageCover;
    }

    public string OrderId { get; set; } = null!;
    public Order Order { get; set; } = null!;

    public string ProductId { get; set; }
    public Product Product { get; set; }
    public string ProductName { get; set; } = null!;
    public string ProductImageCover { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal Total => GetTotal();

    public decimal GetTotal() => UnitPrice * Quantity;
}