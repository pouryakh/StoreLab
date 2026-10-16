using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(18,2);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.Status);
                

            builder.HasIndex(x => x.CreatedAt);

            builder.HasIndex(x => x.PaidAt);

            builder.Property(x => x.OrderId)
                 .IsRequired();

            builder.HasIndex(x => x.OrderId)
                .IsUnique();
        }
    }
}
