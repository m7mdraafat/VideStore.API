using System.ComponentModel.DataAnnotations;

namespace VideStore.Shared.DTOs.Requests.ShoppingCart
{
    public class UpdateCartItemDto
    {
        [Required]
        public string CartItemId { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }

}
