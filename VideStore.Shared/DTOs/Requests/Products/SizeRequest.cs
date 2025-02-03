using System.ComponentModel.DataAnnotations;

namespace VideStore.Shared.DTOs.Requests.Products
{
    public class SizeRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string SizeName { get; set; } = null!;
    }
}
