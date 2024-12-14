using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VideStore.Shared.DTOs.Requests.Categories
{
    public class CategoryRequest
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        public IFormFile? Image { get; set; }
    }
}
