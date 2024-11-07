using System.Text.Json.Serialization;
using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.ProductEntities;

public class Color : BaseEntity
{
    public string ColorName { get; set; } = null!;
    public string ColorHexCode { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Product> Products {get; set; } = [];
}