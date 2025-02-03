
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using VideStore.Domain.Interfaces;
using VideStore.Persistence.Context;
using VideStore.Persistence.Repositories;

namespace VideStore.Persistence;
public static class ServiceExtensions
{
    public static IServiceCollection AddStoreContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<StoreDbContext>(contextOptions =>
        {
            contextOptions.UseSqlServer(connectionString,
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });

        return services;
    }

    public static IServiceCollection AddRedis(this IServiceCollection services, string redisConnectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConnectionString));

        return services;

    }
    public static IServiceCollection AddDistributedCache(this IServiceCollection services, IConfiguration configuration)
    {

        // Caching
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "VideStore_";
        });

        return services;
    }

}