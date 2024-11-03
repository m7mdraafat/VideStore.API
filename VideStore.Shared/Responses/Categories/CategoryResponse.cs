using VideStore.Shared.Responses.Products;

namespace VideStore.Shared.Responses.Categories
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CoverImageUrl { get; set; } = null!;
        public List<ProductResponse> Products { get; set; } = [];
    }
}
