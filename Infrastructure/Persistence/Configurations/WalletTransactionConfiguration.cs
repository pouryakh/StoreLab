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
    public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.Type)
                .IsRequired();

            builder.HasOne(x => x.ReturnRequest)
                .WithOne(x => x.RefundTransaction)
                .HasForeignKey<WalletTransaction>(x => x.ReturnRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ReturnRequestId)
                .IsUnique();

            builder.HasIndex(x => x.OrderId);

            builder.HasIndex(x => x.WalletId);

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
