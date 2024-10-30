
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideStore.Domain.Entities.IdentityEntities;

namespace VideStore.Presistence.Context.Configurations
{
    public class UserAddressConfigurations : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            builder.Property(x => x.AddressName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.AddressLine).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Governorate).IsRequired().HasMaxLength(200);
            builder.Property(x => x.City).IsRequired().HasMaxLength(100);
            builder.Property(x => x.PostalCode).IsRequired().HasMaxLength(50);
            
            builder.HasOne(u=>u.AppUser)
                   .WithMany(u=>u.UserAddresses)
                   .HasForeignKey(x=>x.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
