using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace VideStore.Domain.Interfaces
{
    public interface ISpecifications<T> where T : class
    {
        // The criteria for filtering the data
        Expression<Func<T, bool>> WhereCriteria { get; set; }

        // A collection of expressions for including related entities
        List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> Includes { get; set; }
        // A collection of string paths for nested includes

        // Criteria for ordering the data in ascending order
        Expression<Func<T, object>> OrderBy { get; set; }

        // Criteria for ordering the data in descending order
        Expression<Func<T, object>> OrderByDesc { get; set; }

        // The number of records to skip (for pagination)
        int Skip { get; set; }

        // The number of records to take (for pagination)
        int Take { get; set; }

        // Indicates whether pagination should be applied
        bool IsPaginationEnabled { get; set; }
    }
}