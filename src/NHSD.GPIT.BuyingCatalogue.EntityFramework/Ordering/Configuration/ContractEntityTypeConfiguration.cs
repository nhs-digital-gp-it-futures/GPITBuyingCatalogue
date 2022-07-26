using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    public class ContractEntityTypeConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.ToTable("Contracts", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_Contracts");

            builder.Property(x => x.OrderId).IsRequired();

            builder.HasOne(x => x.DataProcessingPlan)
                .WithMany(x => x.Contracts)
                .HasForeignKey(x => x.DataProcessingPlanId);

            builder.HasOne(x => x.ImplementationPlan)
                .WithMany(x => x.Contracts)
                .HasForeignKey(x => x.ImplementationPlanId);
        }
    }
}
