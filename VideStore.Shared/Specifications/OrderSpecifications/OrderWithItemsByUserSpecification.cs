
using Microsoft.EntityFrameworkCore;
using VideStore.Domain.Entities.OrderEntities;

namespace VideStore.Shared.Specifications.OrderSpecifications
{
    public class OrderWithItemsByUserSpecification : BaseSpecifications<Order>
    {
        public OrderWithItemsByUserSpecification(string userId)
        {
            WhereCriteria = p => p.UserId == userId;
            Includes.Add(q => q.Include(o => o.Items));
            OrderByDesc = p => p.OrderDate;
        }
    }
}
