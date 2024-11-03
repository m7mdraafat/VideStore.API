using Microsoft.EntityFrameworkCore;
using VideStore.Persistence.Context;

namespace VideStore.Infrastructure.Utilities
{
    public class DataDeletionJob(StoreDbContext storeDbContext)
    {
        public async Task Execute()
        {
            await storeDbContext.IdentityCodes
                .Where(p => p.IsActive == false && p.CreationTime < DateTime.UtcNow.AddMinutes(-5))
                .ForEachAsync(p => storeDbContext.IdentityCodes.Remove(p));

            await storeDbContext.SaveChangesAsync();
        }
    }
}
