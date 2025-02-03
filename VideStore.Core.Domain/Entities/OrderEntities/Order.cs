
using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.OrderEntities
{
    public class Order : BaseEntity
    {
        public string UserId { get; private set; }
        public DateTime OrderDate { get; private set; } = DateTime.UtcNow;
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; private set; }
        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        // Required by EF
        private Order() { }

        public Order(string userId, Address shippingAddress, List<OrderItem> items)
        {
            UserId = userId;
            ShippingAddress = shippingAddress;
            _items = items;
        }

        public void UpdateStatus(OrderStatus newStatus) => Status = newStatus;
    }
}
