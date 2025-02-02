using Microsoft.EntityFrameworkCore;
using VideStore.Domain.Entities.ProductEntities;

namespace VideStore.Domain.Specifications.ProductSpecifications
{
    public class ProductWithAllRelatedDataSpecifications : BaseSpecifications<Product>
    {
        public ProductWithAllRelatedDataSpecifications(ProductSpecifications specifications)
        {
            // Add includes for related entities
            Includes.Add(q => q.Include(p => p.Category));
            Includes.Add(q => q.Include(p => p.ProductImages));
            Includes.Add(q => q.Include(p => p.Color));
            Includes.Add(q => q.Include(p => p.ProductSizes).ThenInclude(ps => ps.Size));

            // Set filtering criteria
            WhereCriteria =
                p => (string.IsNullOrEmpty(specifications.Search) ||
                      p.Name.ToLower().Contains(specifications.Search.ToLower())) &&
                     (string.IsNullOrEmpty(specifications.CategoryId) || p.CategoryId == specifications.CategoryId);

            // Apply sorting logic
            if (!string.IsNullOrEmpty(specifications.Sort))
            {
                switch (specifications.Sort)
                {
                    case "name":
                        SetOrderBy(p => p.Name);
                        break;
                    case "nameDesc":
                        SetOrderByDesc(p => p.Name);
                        break;
                    case "price":
                        SetOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        SetOrderByDesc(p => p.Price);
                        break;
                    case "sold":
                        SetOrderByDesc(p => p.Sold);
                        break;
                    case "newest":
                        SetOrderByDesc(p => p.CreatedAt);
                        break;
                    case "oldest":
                        SetOrderBy(p => p.CreatedAt);
                        break;
                    default:
                        SetOrderBy(p => p.Price);
                        break;
                }
            }
            else
            {
                SetOrderByDesc(p => p.CreatedAt); // newest products.
            }

            // Apply pagination
            ApplyPagination((specifications.PageIndex - 1) * specifications.PageSize, specifications.PageSize);
        }

        public ProductWithAllRelatedDataSpecifications(string id)
        {
            WhereCriteria = p => p.Id == id;
            Includes.Add(q => q.Include(p => p.Category));
            Includes.Add(q => q.Include(p => p.ProductImages));
            Includes.Add(q => q.Include(p => p.Color));
            Includes.Add(q => q.Include(p => p.ProductSizes).ThenInclude(ps => ps.Size));
        }
    }
}
