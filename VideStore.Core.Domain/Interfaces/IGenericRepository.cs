using VideStore.Domain.Common;

namespace VideStore.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAllAsync(ISpecifications<T> spec);
        Task<int> GetCountAsync(ISpecifications<T> spec);
        Task<T?> GetEntityAsync(ISpecifications<T> spec);
        Task<T?> GetEntityAsync(string id);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        public void Update(T entity);
        void Delete(T entity);
    }
}
