using VideStore.Domain.Entities.OrderEntities;

namespace VideStore.Shared.DTOs.Order
{
    public record CreateOrderRequest(Address ShippingAddress);
}
