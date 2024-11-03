using Microsoft.AspNetCore.Http;

namespace VideStore.Shared.Requests.Products
{
    public class ProductRequest
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int ColorId { get; set; }
        public List<IFormFile> ProductImages { get; set; } = [];
        public List<int> SizeIds { get; set; } = [];
    }
}
