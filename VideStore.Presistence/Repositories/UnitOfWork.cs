using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Storage;
using VideStore.Domain.Common;
using VideStore.Domain.Interfaces;
using VideStore.Persistence.Context;

namespace VideStore.Persistence.Repositories
{
    public class UnitOfWork(StoreDbContext storeContext) : IUnitOfWork
    {
        private IDbContextTransaction? _transaction;
        private readonly ConcurrentDictionary<string, object> _repositories = new();

        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            var key = typeof(T).Name;
            return (IGenericRepository<T>)_repositories.GetOrAdd(key, _ => new GenericRepository<T>(storeContext));
        }

        public async Task<int> CompleteAsync() => await storeContext.SaveChangesAsync();

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("Transaction is already started.");
            }
            _transaction = await storeContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction is started.");
            }
            await _transaction.CommitAsync();
           
            _transaction.Dispose();
            _transaction = null;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction is started.");
            }
            await _transaction.RollbackAsync();
            _transaction.Dispose();
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            storeContext.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
            await storeContext.DisposeAsync();
        }

    }
}
