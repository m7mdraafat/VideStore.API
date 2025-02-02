using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VideStore.Shared.DTOs.Requests.Categories
{
    public class CategoryRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters.")]
        public string Description { get; set; } = null!;

        public IFormFile? Image { get; set; }
    }
}
