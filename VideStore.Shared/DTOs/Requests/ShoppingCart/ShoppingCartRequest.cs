namespace VideStore.Shared.DTOs.Requests.ShoppingCart
{
    public class ShoppingCartRequest
    {
        public string? UserId { get; set; }
        public string Id {get; set;} = Guid.NewGuid().ToString();
        public List<CartItemRequest> Items { get; set; } = new List<CartItemRequest>();
    }

}
