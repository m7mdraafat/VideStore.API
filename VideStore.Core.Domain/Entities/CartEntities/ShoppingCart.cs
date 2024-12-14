using VideStore.Domain.Entities.IdentityEntities;

namespace VideStore.Domain.Entities.CartEntities;

public class ShoppingCart
{
    public ShoppingCart()
    {
        Id = Guid.NewGuid().ToString();
    }
    public string Id { get; set; }
    public string? UserId { get; set; }
    public AppUser? User { get; set; }
    public List<CartItem> Items { get; set; } = [];
    public decimal? ShippingPrice { get; set; }
    public decimal Subtotal => Items.Sum(x => x.Total);
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
}