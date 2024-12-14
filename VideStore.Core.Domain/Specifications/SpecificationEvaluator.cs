using VideStore.Domain.Common;
using VideStore.Domain.Interfaces;

namespace VideStore.Domain.Specifications
{
    public static class SpecificationsEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> spec)
        {
            if (inputQuery == null) throw new ArgumentNullException(nameof(inputQuery));
            if (spec == null) throw new ArgumentNullException(nameof(spec));

            var query = inputQuery;

            // Apply filtering
            if (spec.WhereCriteria != null)
            {
                query = query.Where(spec.WhereCriteria);
            }

            // Apply includes
            if (spec.Includes != null && spec.Includes.Any())
            {
                foreach (var include in spec.Includes)
                {
                    query = include(query);
                }
            }

            // Apply ordering
            if (spec.OrderByDesc != null)
            {
                query = query.OrderByDescending(spec.OrderByDesc);
            }
            else if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }

            // Apply pagination if enabled
            if (spec.IsPaginationEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            return query;
        }
    }
}