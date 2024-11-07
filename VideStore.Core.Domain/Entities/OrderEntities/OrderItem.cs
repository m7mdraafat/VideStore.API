using VideStore.Domain.Common;
using VideStore.Domain.Entities.ProductEntities;

namespace VideStore.Domain.Entities.OrderEntities;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal GetTotal() => UnitPrice * Quantity;
}