using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideStore.Domain.Entities.OrderEntities;

namespace Infrastructure.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.Status)
                .HasConversion<string>();

            builder.OwnsOne(o => o.ShippingAddress, a =>
            {
                a.Property(p => p.FullName).HasColumnName("ShippingFullName");
                a.Property(p => p.StreetAddress).HasColumnName("ShippingStreetAddress");
                a.Property(p => p.City).HasColumnName("ShippingCity");
                a.Property(p => p.State).HasColumnName("ShippingState");
                a.Property(p => p.PostalCode).HasColumnName("ShippingPostalCode");
                a.Property(p => p.PhoneNumber).HasColumnName("ShippingPhoneNumber");
            });

            builder.HasOne(o => o.DeliveryMethod)
                .WithMany()
                .HasForeignKey("DeliveryMethodId")
                .IsRequired();

            builder.HasMany(o => o.OrderItems)
                .WithOne()
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}