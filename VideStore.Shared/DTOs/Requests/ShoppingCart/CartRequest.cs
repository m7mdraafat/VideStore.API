using System.ComponentModel.DataAnnotations;

namespace VideStore.Shared.DTOs.Requests.ShoppingCart
{
    public class CartRequest
    {
        [Required]
        [MinLength(1, ErrorMessage = "The cart must contain at least one item.")]
        public List<CartItemRequest> Items { get; set; } = new List<CartItemRequest>();
    }

}
