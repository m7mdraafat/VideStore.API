using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideStore.Domain.Entities.ProductEntities;

namespace VideStore.Persistence.Context.Configurations
{

    public class ProductSizeConfiguration : IEntityTypeConfiguration<ProductSize>
    {
        public void Configure(EntityTypeBuilder<ProductSize> builder)
        {
            // Configure composite primary key
            builder.HasKey(ps => new { ps.ProductId, ps.SizeId });

            // Configure properties
            builder.Property(ps => ps.UnitOfStock)
                .IsRequired();

            // Configure relationships
            builder.HasOne(ps => ps.Product)
                .WithMany(p => p.ProductSizes)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ps => ps.Size)
                .WithMany(s => s.ProductSizes)
                .HasForeignKey(ps => ps.SizeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
