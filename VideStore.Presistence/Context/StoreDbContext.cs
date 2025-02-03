using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VideStore.Domain.Entities.IdentityEntities;
using VideStore.Domain.Entities.ProductEntities;


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

    // Cart 
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }


}