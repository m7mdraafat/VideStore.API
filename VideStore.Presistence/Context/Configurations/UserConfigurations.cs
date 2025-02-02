using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideStore.Domain.Entities.IdentityEntities;

namespace VideStore.Presistence.Context.Configurations
{
    public class UserConfigurations : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(p => p.DisplayName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(p => p.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            // Configure RefreshTokens as an owned entity
            builder.OwnsMany(x => x.RefreshTokens, rt =>
            {
                rt.Property(x => x.Token)
                  .IsRequired();
                rt.Property(x => x.ExpireAt)
                  .IsRequired();
                rt.Property(x => x.CreatedAt)
                  .IsRequired();
                rt.Property(x => x.RevokedAt)
                  .IsRequired(false);
            });
        }
    }
}
