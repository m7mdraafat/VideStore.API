using System.Text.Json.Serialization;
using VideStore.Domain.Common;

// Domain/Entities/Cart.cs
public class Cart : BaseEntity
{
    public string? UserId { get; private set; }
    public Guid? GuestId { get; set; }
    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    private Cart()
    {
        
    }
    [JsonConstructor]
    public Cart(string? userId, Guid? guestId, List<CartItem> items)
    {
        UserId = userId;
        GuestId = guestId;
        Items = items;
    }
    // Authenticated user constructor
    public Cart(string userId)
    {
        UserId = userId;
        Items = new List<CartItem>();
    }

    // Guest constructor
    public Cart(Guid guestId)
    {
        GuestId = guestId;
        Items = new List<CartItem>();
    }

    public void AddItem(string productId, int quantity)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        else
            Items.Add(new CartItem(productId, quantity, Id));
    }

    public void MergeWith(Cart otherCart)
    {
        foreach (var item in otherCart.Items)
            AddItem(item.ProductId, item.Quantity);
    }

    public void Clear()
    {
        Items.Clear();

    }

    public void Remove(CartItem item)
    {
        Items.Remove(item);
    }
}
