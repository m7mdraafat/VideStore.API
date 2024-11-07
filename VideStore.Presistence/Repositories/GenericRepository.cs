using Microsoft.EntityFrameworkCore;
using VideStore.Domain.Common;
using VideStore.Domain.Interfaces;
using VideStore.Persistence.Context;
using VideStore.Presistence.Context;
using VideStore.Shared.Specifications;

namespace VideStore.Persistence.Repositories
{
    public class GenericRepository<T>(StoreDbContext storeContext) : IGenericRepository<T> where T : BaseEntity
    {


        public async Task<IReadOnlyList<T>> GetAllAsync() => await storeContext.Set<T>().ToListAsync();

        public async Task<IReadOnlyList<T>> GetAllAsync(ISpecifications<T> spec)
            => await SpecificationsEvaluator<T>.GetQuery(storeContext.Set<T>(), spec).ToListAsync();

        public async Task<int> GetCountAsync(ISpecifications<T> spec)
            => await SpecificationsEvaluator<T>.GetQuery(storeContext.Set<T>(), spec).CountAsync();

        public async Task<T?> GetEntityAsync(ISpecifications<T> spec)
            => await SpecificationsEvaluator<T>.GetQuery(storeContext.Set<T>(), spec).FirstOrDefaultAsync();

        public async Task<T?> GetEntityAsync(int id) => await storeContext.Set<T>().FindAsync(id);

        public async Task AddAsync(T entity) => await storeContext.Set<T>().AddAsync(entity);
        public void Update(T entity)
        {
            // Check if the entity is being tracked
            var existingEntity = storeContext.Set<T>().Local.FirstOrDefault(e => e.Id == entity.Id);

            if (existingEntity != null)
            {
                // Detach the existing entity if it's already in the local cache
                storeContext.Entry(existingEntity).State = EntityState.Detached;
            }

            // Attach the entity and mark it as modified
            storeContext.Set<T>().Attach(entity);
            storeContext.Entry(entity).State = EntityState.Modified;
        }



        public void Delete(T entity) => storeContext.Set<T>().Remove(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities) => await storeContext.Set<T>().AddRangeAsync(entities);
    }
}
