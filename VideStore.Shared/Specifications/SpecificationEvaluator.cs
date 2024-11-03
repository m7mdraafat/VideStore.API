using Microsoft.EntityFrameworkCore;
using VideStore.Domain.Common;
using VideStore.Domain.Interfaces;

namespace VideStore.Shared.Specifications
{
    public static class SpecificationsEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> spec)
        {
            var query = inputQuery;

            // Apply filtering
            query = query.Where(spec.WhereCriteria);

            // Apply includes
            query = spec.IncludesCriteria.Aggregate(query, (current, include) => current.Include(include));

            // Apply ordering - check OrderByDesc first, then fallback to OrderBy
            if (spec.OrderByDesc != null)
            {
                query = query.OrderByDescending(spec.OrderByDesc);
            }
            else if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }

            // Apply pagination
            if (spec.IsPaginationEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            return query;
        }

    }

}
