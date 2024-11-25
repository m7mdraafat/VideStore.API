using Microsoft.EntityFrameworkCore;
using VideStore.Persistence.Context;

namespace VideStore.Infrastructure.Utilities
{
    public class DataDeletionJob(StoreDbContext storeDbContext)
    {
        public async Task DeleteExpiredIdentityCodes()
        {
            var expiredIdentityCodes = await storeDbContext.IdentityCodes
                .Where(p => !p.IsActive || p.CreationTime.AddMinutes(10) < DateTime.UtcNow)
                .ToListAsync();

            storeDbContext.IdentityCodes.RemoveRange(expiredIdentityCodes);

            await storeDbContext.SaveChangesAsync();
        }

        public async Task DeleteExpiredOrRevokedRefreshTokens()
        {
            // Load only users that have expired or revoked tokens into memory
            var usersWithTokens = storeDbContext.Users
                .Include(u => u.RefreshTokens)
                .AsEnumerable()
                .Where(u => u.RefreshTokens.Any(r => r.IsExpired || r.RevokedAt != null))
                .ToList();

            foreach (var user in usersWithTokens)
            {
                // Filter out expired or revoked tokens from user's RefreshTokens collection
                user.RefreshTokens = user.RefreshTokens
                    .Where(r => r is { IsExpired: false, RevokedAt: null })
                    .ToList();
            }

            await storeDbContext.SaveChangesAsync();
        }
    }
}
