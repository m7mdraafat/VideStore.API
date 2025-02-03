namespace VideStore.Shared.Specifications.ProductSpecifications
{
    public class ProductSpecifications
    {
        private const int MaxPageSize = 10;
        private int _pageSize = 10;
        public int PageIndex { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? _pageSize : value;
        }
        public string? CategoryId { get; set; }
        public string? Sort { get; set; }
        public string? Search { get; set; }
    }
}
