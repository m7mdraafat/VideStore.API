
using Microsoft.EntityFrameworkCore;
using VideStore.Domain.Entities.OrderEntities;

namespace VideStore.Shared.Specifications.OrderSpecifications
{
    public class OrderWithItemsSpecifications : BaseSpecifications<Order>
    {
       
        public OrderWithItemsSpecifications()
        {
            Includes.Add(q => q.Include(o => o.OrderItems));
        }

        public OrderWithItemsSpecifications(int orderId)
        {
            WhereCriteria = p => p.Id == orderId;
            Includes.Add(q => q.Include(o => o.OrderItems));

        }
    }
}
