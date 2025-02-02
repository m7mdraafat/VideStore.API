using System.Text.Json.Serialization;
using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.ProductEntities;

public class Product : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public double RatingsAverage { get; set; } = 0;
    public int Sold { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set;} = DateTime.UtcNow;
    public string CategoryId { get; set; } = null!;
    [JsonIgnore]
    public Category Category { get; set; } = null!;

    public string ColorId { get; set; } = null!;
    public Color Color { get; set; } = null!;

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();

}