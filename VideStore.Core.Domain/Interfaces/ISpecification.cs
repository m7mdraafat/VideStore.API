using System.Linq.Expressions;

namespace VideStore.Domain.Interfaces
{
    public interface ISpecifications<T> where T : class
    {
        Expression<Func<T, bool>> WhereCriteria { get; set; }
        List<Expression<Func<T, object>>> IncludesCriteria { get; set; }
        Expression<Func<T, object>> OrderBy { get; set; }
        Expression<Func<T, object>> OrderByDesc { get; set; }
        int Skip { get; set; }
        int Take { get; set; }
        bool IsPaginationEnabled { get; set; }

    }
}
