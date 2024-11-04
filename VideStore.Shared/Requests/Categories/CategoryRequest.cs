using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VideStore.Shared.Requests.Categories
{
    public class CategoryRequest
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}
