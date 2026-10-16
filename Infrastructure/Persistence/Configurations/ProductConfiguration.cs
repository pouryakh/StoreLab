using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Name);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.StockQuantity)
                .IsRequired();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Price)
                .IsRequired()
                .HasPrecision(18,2);

            builder.Property(x => x.IsActive)
                .IsRequired(true)
                .HasDefaultValue(true);

            builder.Property(x => x.StoreId)
                .IsRequired();

            builder.HasIndex(x => x.StoreId);

            builder.HasOne(x => x.Store)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.StoreId);
        }
    }
}
