namespace VideStore.Domain.Entities.OrderEntities;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled
}