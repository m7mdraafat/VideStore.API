using Microsoft.EntityFrameworkCore;
using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.IdentityEntities
{
    [Owned]
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = null!;
        public DateTime ExpireAt { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpireAt;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }
        public bool IsActive => RevokedAt == null && !IsExpired;
    }
}
