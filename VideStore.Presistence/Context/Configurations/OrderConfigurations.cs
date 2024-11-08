using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideStore.Domain.Entities.OrderEntities;

namespace VideStore.Persistence.Context.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.BuyerEmail)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.OrderDate);

            builder.Property(o => o.Status)
                .HasConversion(
                    os => os.ToString(),
                    os => (OrderStatus)Enum.Parse(typeof(OrderStatus), os))
                .IsRequired();

            builder.OwnsOne(o => o.ShippingAddress, sa =>
            {
                sa.Property(a => a.FullName).IsRequired().HasMaxLength(150);
                sa.Property(a => a.StreetAddress).IsRequired().HasMaxLength(200);
                sa.Property(a => a.City).IsRequired().HasMaxLength(50);
                sa.Property(a => a.Governorate).IsRequired().HasMaxLength(100);
                sa.Property(a => a.PostalCode).IsRequired().HasMaxLength(10);
            });

            builder.HasOne(o => o.DeliveryMethod)
                .WithMany()
                .HasForeignKey(o => o.DeliveryMethodId)
                .IsRequired();

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi=>oi.Order)
                .HasForeignKey(oi=>oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(o => o.SubTotal)
                .HasColumnType("decimal(18,2)");
        }


    }
}