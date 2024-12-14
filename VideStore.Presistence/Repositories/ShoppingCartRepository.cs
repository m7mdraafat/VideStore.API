using System.Text.Json;
using StackExchange.Redis;
using VideStore.Domain.Entities.CartEntities;
using VideStore.Domain.Interfaces;

namespace VideStore.Persistence.Repositories
{
    public class ShoppingCartRepository(IConnectionMultiplexer connection) : IShoppingCartRepository
    {

        private readonly IDatabase _database = connection.GetDatabase(); 

        public async Task<ShoppingCart?> CreateOrUpdateShoppingCartAsync(ShoppingCart cart)
        {
            var createdOrUpdated =
                await _database.StringSetAsync(cart.Id, JsonSerializer.Serialize(cart), TimeSpan.FromDays(90));

            if (createdOrUpdated is false) return null;
            return await GetShoppingCartAsync(cart.Id);
        }

        public async Task<ShoppingCart?> GetShoppingCartAsync(string shoppingCartId)
        {
            var cart = await _database.StringGetAsync(shoppingCartId);

            return cart.IsNullOrEmpty ? null : JsonSerializer.Deserialize<ShoppingCart>(cart!);
        }

        public async Task<bool> DeleteShoppingCartAsync(string shoppingCartId)
        {
            return await _database.KeyDeleteAsync(shoppingCartId);
        }

        
    }
}
