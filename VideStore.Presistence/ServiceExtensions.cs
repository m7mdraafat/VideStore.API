
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using VideStore.Persistence.Context;

namespace VideStore.Persistence;
public static class ServiceExtensions
{
    public static IServiceCollection AddStoreContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<StoreDbContext>(contextOptions =>
        {
            contextOptions.UseSqlServer(connectionString);
        });

        return services;
    }

    public static IServiceCollection AddRedis(this IServiceCollection services, string redisConnectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConnectionString));

        return services;

    }


}