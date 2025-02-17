using VideStore.Domain.Entities.OrderEntities;

namespace VideStore.Shared.DTOs.Order
{
    public record OrderDto(
    string Id,
    DateTime OrderDate,
    string Status,
    Address ShippingAddress,
    List<OrderItemDto> Items,
    decimal Total);
}
