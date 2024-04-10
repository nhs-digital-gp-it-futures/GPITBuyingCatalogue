using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class OrderEventEntityTypeConfiguration : IEntityTypeConfiguration<OrderEvent>
    {
        public void Configure(EntityTypeBuilder<OrderEvent> builder)
        {
            builder.ToTable("OrderEvents", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_OrderEvents");

            builder.HasOne(o => o.EventType)
                .WithMany()
                .HasForeignKey(o => o.EventTypeId)
                .HasConstraintName("FK_OrderEvents_Event");
        }
    }
}
