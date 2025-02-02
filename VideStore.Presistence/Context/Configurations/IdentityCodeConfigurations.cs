
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideStore.Domain.Entities.IdentityEntities;

namespace VideStore.Persistence.Context.Configurations
{
    public class IdentityCodeConfigurations : IEntityTypeConfiguration<IdentityCode>
    {
        public void Configure(EntityTypeBuilder<IdentityCode> builder)
        {
            builder.HasKey(ic => ic.Id);
            builder.Property(ic => ic.Id).ValueGeneratedNever();

            builder.Property(ic => ic.AppUserId)
                   .IsRequired();

            builder.Property(ic => ic.Code)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(ic => ic.IsActive)
                   .IsRequired();

            builder.Property(ic => ic.ForRegisterationConfirmed)
                   .IsRequired();

            builder.Property(ic => ic.CreationTime)
                   .IsRequired();

            builder.Property(ic => ic.ActivationTime);

            builder.HasOne(ic => ic.User)
                   .WithMany(u => u.IdentityCodes)
                   .HasForeignKey(ic => ic.AppUserId);
        }
    }
}
