using System.Linq.Expressions;
using VideStore.Domain.Common;
using VideStore.Domain.Interfaces;

namespace VideStore.Shared.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> WhereCriteria { get; set; } = _ => true;
        public List<Expression<Func<T, object>>> IncludesCriteria { get; set; } = [];
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDesc { get; set; } 
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPaginationEnabled { get; set; }

        // Fluent API for adding includes
        public BaseSpecifications<T> Include(Expression<Func<T, object>> includeExpression)
        {
            IncludesCriteria.Add(includeExpression);
            return this;
        }

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