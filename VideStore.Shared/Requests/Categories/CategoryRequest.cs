namespace VideStore.Shared.Requests.Categories
{
    public class CategoryRequest
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CoverImageUrl { get; set; } = null!;
    }
}
