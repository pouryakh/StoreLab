using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class StoreActivationRequestConfiguration
        : IEntityTypeConfiguration<StoreActivationRequest>
    {
        public void Configure(EntityTypeBuilder<StoreActivationRequest> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.StoreId)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();
                

            builder.Property(x => x.AdminNote)
                .HasMaxLength(1000);

            builder.Property(x => x.ReviewedAt);

            builder.HasIndex(x => x.StoreId);

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => x.ReviewedAt);
        }
    }
}