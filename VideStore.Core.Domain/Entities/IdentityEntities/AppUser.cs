using Microsoft.AspNetCore.Identity;

namespace VideStore.Domain.Entities.IdentityEntities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = null!;
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];
        public virtual ICollection<UserAddress> UserAddresses { get; set; } = []; 
    }
}
