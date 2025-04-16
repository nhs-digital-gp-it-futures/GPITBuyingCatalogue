using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal class OrderSublocationEntityTypeConfiguration : IEntityTypeConfiguration<OrderSublocation>
    {
        public void Configure(EntityTypeBuilder<OrderSublocation> builder)
        {
            builder.ToTable("OrderSublocations", Schemas.Ordering);

            builder.HasKey(x => new { x.OrderId, x.SublocationOdsCode });

            builder.Property(x => x.OrderId).IsRequired();

            builder.Property(x => x.SublocationOdsCode).HasMaxLength(10).IsRequired();

            builder.Property(x => x.OwnerOdsCode).HasMaxLength(10).IsRequired();

            builder.HasOne(x => x.Order)
                .WithMany(y => y.OrderSublocations)
                .HasForeignKey(x => x.OrderId)
                .HasConstraintName("FK_OrderSublocations_Order");

            builder.HasMany(x => x.SublocationRecipients)
                .WithOne(y => y.ParentSublocation)
                .HasForeignKey(y => y.ParentSublocationOdsCode)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderSublocationRecipients_ParentSublocation");

            builder.HasOne(x => x.SublocationOrganisation)
                .WithMany()
                .HasForeignKey(x => x.SublocationOdsCode)
                .HasConstraintName("FK_OrderSublocations_OdsOrganisations_Sublocation");
        }
    }
}
