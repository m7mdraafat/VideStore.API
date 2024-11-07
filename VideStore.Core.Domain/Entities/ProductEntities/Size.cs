using System.Text.Json.Serialization;
using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.ProductEntities;

public class Size : BaseEntity
{
    public string SizeName { get; set; } = string.Empty;

    [JsonIgnore]
    public virtual ICollection<ProductSize> ProductSizes { get; set; } = [];

}