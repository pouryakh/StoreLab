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
    public class StoreOrderConfiguration : IEntityTypeConfiguration<StoreOrder>
    {
        public void Configure(EntityTypeBuilder<StoreOrder> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.DeliveredAt);

            builder.HasIndex(x => x.SettlementEligibleAt);

            builder.Property(x => x.TotalAmount)
                .HasPrecision(18,2)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();


            builder.Property(x => x.SettlementStatus)
                .IsRequired();
                

            builder.HasOne(x => x.Store)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
