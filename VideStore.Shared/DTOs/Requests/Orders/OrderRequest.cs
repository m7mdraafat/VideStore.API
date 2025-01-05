using VideStore.Domain.Entities.OrderEntities;

namespace VideStore.Shared.DTOs.Requests.Orders
{
    public class OrderRequest
    {
        public OrderAddress ShippingAddress { get; set; } = null!;

        public ICollection<OrderItemRequest> OrderItems { get; set; } = new List<OrderItemRequest>();

        public decimal SubTotal { get; set; }
    }
}
