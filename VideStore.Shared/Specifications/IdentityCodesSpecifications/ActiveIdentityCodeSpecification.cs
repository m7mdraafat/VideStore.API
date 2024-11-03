using VideStore.Domain.Entities.IdentityEntities;

namespace VideStore.Shared.Specifications.IdentityCodesSpecifications
{
    public class ActiveIdentityCodeSpecification : BaseSpecifications<IdentityCode>
    {
        public ActiveIdentityCodeSpecification(string userId, bool forRegister)
        {
            WhereCriteria = code => code.AppUserId == userId && code.ForRegisterationConfirmed == forRegister;
            OrderByDesc = code => code.CreationTime; // Ordering by creation time
        }
    }
}
