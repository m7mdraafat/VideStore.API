using Microsoft.EntityFrameworkCore;
using VideStore.Domain.Common;
using VideStore.Domain.Interfaces;
using VideStore.Persistence.Context;
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
            // Attach the entity only if it's not already tracked by the context
            var existingEntity = storeContext.Set<T>().Find(entity.Id);

            if (existingEntity == null)
            {
                // If the entity is not found in the context, attach and set as modified
                storeContext.Set<T>().Attach(entity);
            }
            else
            {
                // If the entity is already tracked, update its properties directly
                storeContext.Entry(existingEntity).CurrentValues.SetValues(entity);
            }

            // Mark the entity as modified (if necessary)
            storeContext.Entry(entity).State = EntityState.Modified;
        }




        public void Delete(T entity) => storeContext.Set<T>().Remove(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities) => await storeContext.Set<T>().AddRangeAsync(entities);
    }
}
