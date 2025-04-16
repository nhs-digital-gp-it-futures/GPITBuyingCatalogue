using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal class
        OrderSublocationRecipientEntityTypeConfiguration : IEntityTypeConfiguration<OrderSublocationRecipient>
    {
        public void Configure(EntityTypeBuilder<OrderSublocationRecipient> builder)
        {
            builder.ToTable("OrderSublocationRecipients", Schemas.Ordering);

            builder.HasKey(x => new { x.OrderId, x.RecipientOdsCode });

            builder.Property(x => x.OrderId).IsRequired();

            builder.Property(x => x.RecipientOdsCode).HasMaxLength(10).IsRequired();

            builder.Property(x => x.ParentSublocationOdsCode).HasMaxLength(10).IsRequired();

            builder.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .HasConstraintName("FK_OrderSublocationRecipients_Order");

            builder.HasOne(x => x.ParentSublocation)
                .WithMany(y => y.SublocationRecipients)
                .HasForeignKey(x => x.ParentSublocationOdsCode)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderSublocationRecipients_ParentSublocation");

            builder.HasOne(x => x.RecipientOdsOrganisation)
                .WithMany()
                .HasForeignKey(x => x.RecipientOdsCode)
                .HasConstraintName("FK_OrderSublocationRecipients_OdsOrganisations_Recipient");

            builder.HasMany(x => x.OrderItemSublocationRecipients)
                .WithOne(y => y.Recipient)
                .HasForeignKey(y => y.OdsCode)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderItemSublocationRecipients_SublocationRecipient");
        }
    }
}
