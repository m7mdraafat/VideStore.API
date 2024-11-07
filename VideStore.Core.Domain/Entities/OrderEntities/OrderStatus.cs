namespace VideStore.Domain.Entities.OrderEntities;

public enum OrderStatus
{
    Pending=1,
    Processing=2,
    Shipped=3,
    Completed=4,
    Cancelled =5
}