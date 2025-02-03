
using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.OrderEntities
{

    public class OrderItem : BaseEntity
    {
        public string ProductId { get; private set; }
        public string ProductName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public string PictureUrl { get; private set; }

        private OrderItem() { }

        public OrderItem(string productId, string productName, decimal unitPrice,
                       int quantity, string pictureUrl)
        {
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
            PictureUrl = pictureUrl;
        }
    }
}
