using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace VideStore.Shared.DTOs.Requests.Products
{
    public class ProductRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int StockQuantity { get; set; }

        [Required]
        public string CategoryId { get; set; } = null!;

        [Required]
        public string ColorId { get; set; } = null!;

        [Required(ErrorMessage = "At least one product image is required.")]
        public List<IFormFile> ProductImages { get; set; } = new List<IFormFile>();

        [Required(ErrorMessage = "Sizes is required.")]
        public List<string> SizeIds { get; set; } = new List<string>();
    }
}
