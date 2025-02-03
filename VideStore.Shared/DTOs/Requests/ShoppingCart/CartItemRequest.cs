using System.ComponentModel.DataAnnotations;

namespace VideStore.Shared.DTOs.Requests.ShoppingCart
{
    public class CartItemRequest
    {
        [Required]
        public string ProductId { get; set; } = null!;

        [Required]
        public string CartId { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal PriceAtAddition { get; set; }

        [Required]
        public DateTime AddedAt { get; set; }
    }

}
