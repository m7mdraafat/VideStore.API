using System.Text.Json.Serialization;
using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.ProductEntities;

public class ProductSize : BaseEntity
{
    public string ProductId { get; set; } = null!;
    [JsonIgnore]
    public Product Product { get; set; } = null!;
    public string SizeId { get; set; } = null!;
    public Size Size { get; set; } = null!;

}