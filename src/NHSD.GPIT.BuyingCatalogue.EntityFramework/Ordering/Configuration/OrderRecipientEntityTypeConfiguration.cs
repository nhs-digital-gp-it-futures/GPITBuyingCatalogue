using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration;

public sealed class OrderRecipientEntityTypeConfiguration : IEntityTypeConfiguration<OrderRecipient>
{
    public void Configure(EntityTypeBuilder<OrderRecipient> builder)
    {
        builder.ToTable("OrderRecipients", Schemas.Ordering);

        builder.HasKey(x => new { x.OrderId, x.OdsCode });

        builder.Property(x => x.OrderId).IsRequired();

        builder.Property(x => x.OdsCode).IsRequired();

        builder.HasOne(x => x.OdsOrganisation)
            .WithMany()
            .HasForeignKey(x => x.OdsCode)
            .HasConstraintName("FK_OrderRecipients_ServiceRecipient");

        builder.HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .HasConstraintName("FK_OrderRecipients_Orders");
    }
}
