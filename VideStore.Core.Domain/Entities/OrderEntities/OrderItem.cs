using VideStore.Domain.Common;
using VideStore.Domain.Entities.ProductEntities;

namespace VideStore.Domain.Entities.OrderEntities;

public class OrderItem : BaseEntity
{
    public OrderItem() { }

    public OrderItem(int orderId, int productId, Product product, int quantity, decimal unitPrice)
    {
        OrderId = orderId;
        ProductId = productId;
        Product = product;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal Total => GetTotal();

    public decimal GetTotal() => UnitPrice * Quantity;
}