using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using VideStore.Domain.Common;
using VideStore.Domain.Interfaces;

namespace VideStore.Domain.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> WhereCriteria { get; set; }
        public List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> Includes { get; set; } = new List<Func<IQueryable<T>, IIncludableQueryable<T, object>>>(); // Correct initialization
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDesc { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPaginationEnabled { get; set; }

        // Fluent API for setting order by
        public BaseSpecifications<T> SetOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
            return this;
        }

        // Fluent API for setting order by descending
        public BaseSpecifications<T> SetOrderByDesc(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDesc = orderByDescExpression;
            return this;
        }

        public void ApplyPagination(int skip, int take)
        {
            IsPaginationEnabled = true;
            Skip = skip;
            Take = take;
        }
    }
}