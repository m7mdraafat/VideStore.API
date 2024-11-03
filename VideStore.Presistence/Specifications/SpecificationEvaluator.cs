using Microsoft.EntityFrameworkCore;
using VideStore.Domain.Common;
using VideStore.Domain.Interfaces;

namespace VideStore.Persistence.Specifications
{
    public static class SpecificationsEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> spec)
        {
            var query = inputQuery;

            // Apply filtering
            if(spec.WhereCriteria != null)
                query = query.Where(spec.WhereCriteria);

            // Apply includes
            if (spec.IncludesCriteria != null)
                query = spec.IncludesCriteria.Aggregate(query, (current, include) => current.Include(include));

            // Apply ordering
            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDesc != null)
                query = query.OrderByDescending(spec.OrderByDesc);

            // Apply pagination
            if (spec.IsPaginationEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            return query;
        }
    }

}
