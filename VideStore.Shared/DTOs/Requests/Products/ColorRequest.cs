using System.ComponentModel.DataAnnotations;

namespace VideStore.Shared.DTOs.Requests.Products
{
    public class ColorRequest
    {
        [Required(ErrorMessage = "Color name is required.")]
        [StringLength(50, ErrorMessage = "Color name cannot be longer than 50 characters.")]
        public string ColorName { get; set; } = null!;

        [Required(ErrorMessage = "Color hex code is required.")]
        [RegularExpression("^#([A-Fa-f0-9]{6})$", ErrorMessage = "Invalid color hex code.")]
        public string ColorHexCode { get; set; } = null!;
    }
}
