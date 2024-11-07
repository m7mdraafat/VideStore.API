using System.Text.Json.Serialization;
using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.ProductEntities;

public class ProductSize : BaseEntity
{
    public int ProductId { get; set; }
    [JsonIgnore]
    public Product Product { get; set; } = null!;
    public int SizeId { get; set; }
    public Size Size { get; set; } = null!;

}