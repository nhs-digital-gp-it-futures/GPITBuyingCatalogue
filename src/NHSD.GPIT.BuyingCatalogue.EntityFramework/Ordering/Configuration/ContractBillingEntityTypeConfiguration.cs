using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    public class ContractBillingEntityTypeConfiguration : IEntityTypeConfiguration<ContractBilling>
    {
        public void Configure(EntityTypeBuilder<ContractBilling> builder)
        {
            builder.ToTable("ContractBilling", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_ContractBilling");

            builder.Property(x => x.ContractId).IsRequired();

            builder.HasOne(x => x.Contract)
                .WithOne(y => y.ContractBilling)
                .HasForeignKey<ContractBilling>(x => x.ContractId)
                .HasConstraintName("FK_ContractBilling_Contract")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ContractBillingItems)
                .WithOne(x => x.ContractBilling)
                .HasForeignKey(x => x.ContractBillingId)
                .HasConstraintName("FK_ContractBillingItems_ContractBilling");

            builder.HasMany(x => x.Requirements)
                .WithOne(x => x.ContractBilling)
                .HasForeignKey(x => x.ContractBillingId)
                .HasConstraintName("FK_Requirements_ContractBilling");
        }
    }
}
