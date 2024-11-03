using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.ProductEntities;

public class Product : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int TotalStockQuantity => ProductSizes.Sum(ps => ps.UnitOfStock); 
    public double RatingsAverage { get; set; } = 0;
    public int Sold { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set;} = DateTime.UtcNow;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int ColorId { get; set; }
    public Color Color { get; set; } = null!;

    public virtual ICollection<ProductImage> ProductImages { get; set; } = [];
    public virtual ICollection<ProductSize> ProductSizes { get; set; } = [];

}