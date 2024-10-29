
using VideStore.Domain.Common;
namespace VideStore.Domain.Entities.IdentityEntities
{
    public class IdentityCode: BaseEntity
    {
        public string AppUserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        public string Code { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool ForRegisterationConfirmed { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public DateTime? ActivationTime { get; set; }
    }
}
