using Microsoft.AspNetCore.Identity;
using VideStore.Domain.Entities.OrderEntities;

namespace VideStore.Domain.Entities.IdentityEntities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = null!;
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
        public virtual ICollection<Order>? Orders { get; set; } = new List<Order>();

    }
}
