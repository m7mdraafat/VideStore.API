using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideStore.Domain.Entities.OrderEntities;

namespace VideStore.Persistence.Context.Configurations;

public class OrderDeliveryMethodConfiguration : IEntityTypeConfiguration<OrderDeliveryMethod>
{
    public void Configure(EntityTypeBuilder<OrderDeliveryMethod> builder)
    {
        builder.Property(dm => dm.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(dm => dm.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(dm => dm.Cost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}