using VideStore.Domain.Common;
using VideStore.Domain.Entities.IdentityEntities;

namespace VideStore.Domain.Entities.OrderEntities;

public class Order : BaseEntity
{
    #region Constructors
    public Order() { }

    public Order(string userId, OrderAddress shippingAddress, ICollection<OrderItem> items, decimal subTotal)
    {
        UserId = userId;
        ShippingAddress = shippingAddress;
        OrderItems = items;
        SubTotal = subTotal;
        OrderDate = DateTimeOffset.UtcNow;
    }

    #endregion

    #region Properties
    public string UserId { get; set; } = null!;
    public AppUser? AppUser { get; set; } // Optional to avoid circular dependencies
    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTime? ShippingDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public OrderAddress ShippingAddress { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
    public decimal SubTotal { get; set; }
    public decimal GetTotal() => SubTotal + DeliveryFee;
    public decimal DeliveryFee => CalculateDeliveryFee(ShippingAddress.Governorate);
    #endregion

    #region helper methods
    private decimal CalculateDeliveryFee(string governorate)
    {
        var deliveryFees = new Dictionary<string, decimal>
        {
            { "Cairo", 30.00M },
            { "Giza", 35.00M },
            { "Alexandria", 50.00M },
            { "Aswan", 60.00M },
            { "Asyut", 55.00M },
            { "Beheira", 45.00M },
            { "Beni Suef", 40.00M },
            { "Dakahlia", 40.00M },
            { "Damietta", 45.00M },
            { "Faiyum", 35.00M },
            { "Gharbia", 40.00M },
            { "Ismailia", 50.00M },
            { "Kafr El Sheikh", 50.00M },
            { "Luxor", 55.00M },
            { "Matrouh", 70.00M },
            { "Minya", 50.00M },
            { "Monufia", 45.00M },
            { "New Valley", 75.00M },
            { "North Sinai", 80.00M },
            { "Port Said", 40.00M },
            { "Qalyubia", 30.00M },
            { "Qena", 55.00M },
            { "Red Sea", 75.00M },
            { "Sharqia", 40.00M },
            { "Sohag", 60.00M },
            { "South Sinai", 85.00M },
            { "Suez", 50.00M }
        };

        return deliveryFees.GetValueOrDefault(governorate, 100.00M);
    }
    #endregion
}
