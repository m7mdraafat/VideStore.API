using VideStore.Domain.Entities.IdentityEntities;

namespace VideStore.Shared.Specifications
{
    public class ActiveIdentityCodeSpecification : BaseSpecifications<IdentityCode>
    {
        public ActiveIdentityCodeSpecification(string userId)
        {
            WhereCriteria = code => code.AppUserId == userId && code.ForRegisterationConfirmed;
            OrderByDesc = code => code.CreationTime; // Ordering by creation time
        }
    }
}
