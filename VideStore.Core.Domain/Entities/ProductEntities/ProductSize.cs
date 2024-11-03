using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.ProductEntities;

public class ProductSize : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public int SizeId { get; set; }
    public Size Size { get; set; } = null!;

    public int UnitOfStock { get; set; }
}