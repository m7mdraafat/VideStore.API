﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VideStore.Presistence.Context;

namespace VideStore.Presistence;
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

}