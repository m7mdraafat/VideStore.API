using VideStore.Shared.DTOs.Requests.Orders;

namespace VideStore.Shared.DTOs.Responses.Orders
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public string BuyerEmail { get; set; } = null!;
        public DateTimeOffset OrderDate { get; set; }
        public string OrderStatus { get; set; } = null!;
        public OrderAddressDto ShippingAddress { get; set; } = null!;
        public decimal DeliveryFee { get; set; }
        public ICollection<OrderItemResponse> OrderItems { get; set; } = new List<OrderItemResponse>();
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
    }
}
