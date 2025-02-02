using System.Text.Json.Serialization;
using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.ProductEntities;

public class ProductImage : BaseEntity
{
    public string ImageUrl { get; set; } = null!;

    public string ProductId { get; set; } = null!;
    [JsonIgnore]
    public Product Product { get; set; } = null!;

}