using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideStore.Domain.Entities.ProductEntities;

namespace VideStore.Persistence.Context.Configurations
{

    public class SizeConfiguration : IEntityTypeConfiguration<Size>
    {
        public void Configure(EntityTypeBuilder<Size> builder)
        {
            // Configure primary key
            builder.HasKey(s => s.Id);

            // Configure properties
            builder.Property(s => s.SizeName)
                .IsRequired()
                .HasMaxLength(50);

            // Configure relationships
            builder.HasMany(s => s.ProductSizes)
                .WithOne(ps => ps.Size)
                .HasForeignKey(ps => ps.SizeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
