using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.OrderEntities;

public class OrderDeliveryMethod : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Cost { get; set; }
}