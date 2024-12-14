namespace VideStore.Shared.DTOs.Requests.Orders
{
    public class OrderRequest
    {
        public string BuyerEmail { get; set; } = null!;

        public OrderAddressDto ShippingAddress { get; set; } = null!;

        public ICollection<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();

        public decimal SubTotal { get; set; }
    }
}
