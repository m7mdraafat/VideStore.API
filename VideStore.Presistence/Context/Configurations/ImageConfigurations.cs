using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideStore.Domain.Entities.ProductEntities;

namespace VideStore.Persistence.Context.Configurations
{

    public class ImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            // Configure primary key
            builder.HasKey(i => i.Id);

            // Configure properties
            builder.Property(i => i.ImageUrl)
                .IsRequired()
                .HasMaxLength(250);

            // Configure relationships
            builder.HasOne(i => i.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
