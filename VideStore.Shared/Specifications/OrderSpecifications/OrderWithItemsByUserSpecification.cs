
//using Microsoft.EntityFrameworkCore;
//using VideStore.Domain.Entities.OrderEntities;

//namespace VideStore.Shared.Specifications.OrderSpecifications
//{
//    public class OrderWithItemsByUserSpecification : BaseSpecifications<Order>
//    {
//        public OrderWithItemsByUserSpecification(string? buyerEmail)
//        {
//            WhereCriteria = p => p.BuyerEmail == buyerEmail;
//            Includes.Add(q => q.Include(o => o.OrderItems));
//            OrderBy = p => p.OrderDate;
//        }
//    }
//}
