using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ReturnRequestConfiguration : IEntityTypeConfiguration<ReturnRequest>
    {
        public void Configure(EntityTypeBuilder<ReturnRequest> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.StoreOrderId)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Reason)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.Status)
                .IsRequired();
                

            builder.Property(x => x.RequestedAt)
                .IsRequired();

            builder.Property(x => x.ResolvedAt);

            builder.Property(x => x.ReceivedAt);

            builder.HasIndex(x => x.StoreOrderId);

            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.RequestedAt);
        }
    }
}