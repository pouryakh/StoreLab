using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TicketMessageConfiguration : IEntityTypeConfiguration<TicketMessage>
    {
        public void Configure(EntityTypeBuilder<TicketMessage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TicketId)
                .IsRequired();

            builder.Property(x => x.SenderId)
                .IsRequired();

            builder.Property(x => x.Message)
                .IsRequired()
                .HasMaxLength(2000);

            builder.HasIndex(x => x.TicketId);

            builder.HasIndex(x => x.SenderId);

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}