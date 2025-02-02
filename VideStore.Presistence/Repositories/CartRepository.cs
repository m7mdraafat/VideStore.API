using Microsoft.EntityFrameworkCore;
using System;
using VideStore.Domain.Interfaces;
using VideStore.Persistence.Context;

namespace VideStore.Persistence.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly StoreDbContext _dbContext;
        public CartRepository(StoreDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<Cart?> GetByUserIdAsync(string userId, bool includeItems = true)
        {
            var query = includeItems
                ? _dbContext.Carts.Include(c => c.Items)
                : _dbContext.Carts.AsQueryable();

            return await query.FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetByGuestIdAsync(Guid guestId, bool includeItems = true)
        {
            var query = includeItems
                ? _dbContext.Carts.Include(c => c.Items)
                : _dbContext.Carts.AsQueryable();

            return await query.FirstOrDefaultAsync(c => c.GuestId == guestId);
        }
    }
}
