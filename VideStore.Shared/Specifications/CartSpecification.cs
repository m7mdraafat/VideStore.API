using Microsoft.EntityFrameworkCore;

namespace VideStore.Shared.Specifications
{
    public class CartSpecification : BaseSpecifications<Cart>
    {
        public CartSpecification()
        {
            
        }
        public CartSpecification(string userId)
        {
            WhereCriteria = c => c.UserId == userId;
            Includes.Add(q=>q.Include(c=>c.Items)); 
        }

        public CartSpecification(Guid guestId)
        {
            WhereCriteria = c => c.GuestId == guestId;
            Includes.Add(q => q.Include(c => c.Items));
        }
    }
}
