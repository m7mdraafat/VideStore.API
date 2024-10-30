using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VideStore.Domain.Entities.IdentityEntities;
using VideStore.Presistence.Context.Configurations;

namespace VideStore.Presistence.Context;

public class StoreDbContext(DbContextOptions<StoreDbContext> options):  IdentityDbContext<AppUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(UserConfigurations).Assembly);
    }

    public DbSet<IdentityCode> IdentityCodes { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<UserAddress> UserAddresses { get; set; }
}