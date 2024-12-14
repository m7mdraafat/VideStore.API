using Microsoft.EntityFrameworkCore;
using VideStore.Domain.Entities.ProductEntities;

namespace VideStore.Domain.Specifications.ProductSpecifications
{
    public class ProductCountSpecifications : BaseSpecifications<Product>
    {
        public ProductCountSpecifications(ProductSpecifications specifications)
        {
            Includes.Add(q=>q.Include(p=>p.Category));
            WhereCriteria =
                p => (string.IsNullOrEmpty(specifications.Search) || p.Name.ToLower().Contains(specifications.Search.ToLower())) &&
                     (!specifications.CategoryId.HasValue || p.CategoryId == specifications.CategoryId.Value);
        }
    }
}
