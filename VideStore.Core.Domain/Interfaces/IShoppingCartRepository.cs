using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideStore.Domain.Entities.CartEntities;

namespace VideStore.Domain.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart?> CreateOrUpdateShoppingCartAsync(ShoppingCart  cart);
        Task<ShoppingCart?> GetShoppingCartAsync(string shoppingCartId);
        Task<bool> DeleteShoppingCartAsync(string shoppingCartId); 
    }
}
