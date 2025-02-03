using System.Text.Json.Serialization;
using VideStore.Domain.Common;

public class CartItem : BaseEntity
{
    public string ProductId { get; private set; }
    public int Quantity { get; private set; }
    public string CartId { get; private set; }
    [JsonIgnore]
    public Cart Cart { get; set; }
    public CartItem(string productId, int quantity, string cartId)
    {
        ProductId = productId;
        Quantity = quantity;
        CartId = cartId;
    }

    public void UpdateQuantity(int newQuantity) => Quantity = newQuantity;
}