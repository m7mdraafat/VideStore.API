using VideStore.Domain.Entities.IdentityEntities;

namespace VideStore.Persistence.Specifications
{
    public class ActiveIdentityCodeSpecification : BaseSpecifications<IdentityCode>
    {
        public ActiveIdentityCodeSpecification(string userId)
        {
            WhereCriteria = code => code.AppUserId == userId && code.ForRegisterationConfirmed;
            SetOrderByDesc(code => code.CreationTime); 
        }

    }
}
