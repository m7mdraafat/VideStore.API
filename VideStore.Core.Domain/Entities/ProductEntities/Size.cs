using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.ProductEntities;

public class Size : BaseEntity
{
    public string SizeName { get; set; } = string.Empty;

    public virtual ICollection<ProductSize> ProductSizes { get; set; } = [];

}