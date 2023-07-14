using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    public class RequirementEntityTypeConfiguration : IEntityTypeConfiguration<Requirement>
    {
        public void Configure(EntityTypeBuilder<Requirement> builder)
        {
            builder.ToTable("Requirements", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_Requirements");

            builder.Property(x => x.ContractBillingId).IsRequired();
            builder.Property(x => x.OrderId).IsRequired();
            builder.Property(x => x.CatalogueItemId).IsRequired();
            builder.Property(x => x.Details).IsRequired();

            builder.HasOne(x => x.ContractBilling)
                .WithMany(x => x.Requirements)
                .HasForeignKey(x => x.ContractBillingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.OrderItem)
                .WithMany()
                .HasForeignKey(x => new { x.OrderId, x.CatalogueItemId })
                .HasConstraintName("FK_Requirements_OrderItem");
        }
    }
}
