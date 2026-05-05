using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    public class ContractBillingItemEntityTypeConfiguration : IEntityTypeConfiguration<ContractBillingItem>
    {
        public void Configure(EntityTypeBuilder<ContractBillingItem> builder)
        {
            builder.ToTable("ContractBillingItems", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_ContractBillingItems");

            builder.Property(x => x.ContractBillingId).IsRequired();
            builder.Property(x => x.OrderItemId).IsRequired();

            builder.HasOne(x => x.ContractBilling)
                .WithMany(x => x.ContractBillingItems)
                .HasForeignKey(x => x.ContractBillingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.OrderItem)
                .WithMany()
                .HasForeignKey(x => x.OrderItemId)
                .HasConstraintName("FK_ContractBillingItems_OrderItemV2");
        }
    }
}
