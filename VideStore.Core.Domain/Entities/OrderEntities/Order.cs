using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.OrderEntities
{
    public class Order : BaseEntity
    {
        public Order() { /* Required by EF */ }

        public Order(string buyerEmail, OrderAddress shippingAddress, OrderDeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subTotal)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            OrderItems = items;
            SubTotal = subTotal;
        }

        public string BuyerEmail { get; set; } = null!;

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public OrderAddress ShippingAddress { get; set; } = null!;

        public int DeliveryMethodId { get; set; }
        public OrderDeliveryMethod DeliveryMethod { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();

        public decimal SubTotal { get; set; }

        public decimal GetTotal() => SubTotal + DeliveryMethod.Cost;
    }
}