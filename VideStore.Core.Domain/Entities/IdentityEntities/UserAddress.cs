using VideStore.Domain.Common;

namespace VideStore.Domain.Entities.IdentityEntities
{
    public class UserAddress : BaseEntity
    {
        public string AddressName { get; set; } = null!;
        public string AddressLine { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Governorate { get; set; } = null!;
        public string? PostalCode { get; set; }

        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null;
    }
}