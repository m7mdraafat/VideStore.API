﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VideStore.Domain.Entities.CartEntities;
using VideStore.Domain.Entities.IdentityEntities;
using VideStore.Domain.Entities.OrderEntities;
using VideStore.Domain.Entities.ProductEntities;
using VideStore.Presistence.Context.Configurations;

namespace VideStore.Persistence.Context;

public class StoreDbContext(DbContextOptions<StoreDbContext> options):  IdentityDbContext<AppUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        //builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyConfigurationsFromAssembly(typeof(StoreDbContext).Assembly);
    }

    // users 
    public DbSet<IdentityCode> IdentityCodes { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<UserAddress> UserAddresses { get; set; }

    // products
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories {get; set; }
    public DbSet<Size> Sizes { get; set; }
    public DbSet<ProductSize> ProductSizes { get; set; }
    public DbSet<Color> Colors { get; set; }


    // order
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }


}